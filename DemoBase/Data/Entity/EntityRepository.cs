using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace DemoBase.Data.Entity
{
    public class EntityRepository<T> : IRepository<T>
        where T : class
    {
        private readonly DbContextBase _context;
        private IDbSet<T> _entities;

        public EntityRepository(DbContextBase context)
        {
            this._context = context;
        }

        public DbContextBase Context { get { return _context; } }

        public virtual IQueryable<T> Table
        {
            get { return Entities; }
        }

        public IQueryable<TTable> GetTable<TTable>() where TTable : class
        {
            return _context.Set<TTable>();
        }

        protected IDbSet<T> Entities
        {
            get { return _entities ?? (_entities = _context.Set<T>()); }
        }

        public int Count()
        {
            return Entities.Count();
        }

        public void Delete(T entity, bool deleteRelated = false)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                if (deleteRelated)
                {
                    var objectContext = ((IObjectContextAdapter)_context).ObjectContext;
                    var type = typeof(T);
                    var properties = type.GetProperties();
                    var edmType = GetEdmType(objectContext.MetadataWorkspace, typeof(T));

                    foreach (NavigationProperty member in edmType.Members.Where(x => x.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty))
                    {
                        if (member.FromEndMember.RelationshipMultiplicity == RelationshipMultiplicity.One
                            && member.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
                        {
                            var property = properties.First(x => x.Name == member.Name);
                            var items = property.GetValue(entity) as IEnumerable<object>;

                            if (items == null)
                            {
                                _context.Entry(entity).Collection(member.Name).Load();
                                items = property.GetValue(entity) as IEnumerable<object>;
                            }

                            if (items != null)
                            {
                                while (items.Any())
                                {
                                    _context.Entry(items.ElementAt(0)).State = EntityState.Deleted;
                                }
                            }
                        }
                    }
                }

                _context.Entry(entity).State = EntityState.Deleted;
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
            }
        }

        public void Delete(Expression<Func<T, bool>> filterExpression)
        {
            try
            {
                //Table.Where(filterExpression).Delete();
            }
            catch
            {
                var items = Table.Where(filterExpression).ToList();
                DeleteMany(items);
            }
        }

        public void DeleteMany(IEnumerable<T> items)
        {
            if (!items.Any())
            {
                return;
            }

            try
            {
                var collection = new List<T>(items);
                foreach (var item in collection)
                {
                    _context.Entry(item).State = EntityState.Deleted;
                }
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = dbEx.EntityValidationErrors
                    .SelectMany(validationErrors => validationErrors.ValidationErrors)
                    .Aggregate(string.Empty, (current, validationError) => current + (System.Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage)));

                throw new Exception(msg, dbEx);
            }
        }

        public virtual T GetById(object id)
        {
            return Entities.Find(id);
        }

        public void Insert(T entity, params Expression<Func<T, dynamic>>[] includePaths)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                Entities.Add(entity);
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = dbEx.EntityValidationErrors
                    .SelectMany(validationErrors => validationErrors.ValidationErrors)
                    .Aggregate(string.Empty, (current, validationError) => current + (string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + System.Environment.NewLine));

                throw new Exception(msg, dbEx);
            }
        }

        public void InsertMany(IEnumerable<T> items)
        {
            if (!items.Any())
            {
                return;
            }

            try
            {
                var sqlConnection = _context.Database.Connection as SqlConnection;
                if (sqlConnection != null)
                {
                    using (var bulkInsert = new SqlBulkCopy(sqlConnection))
                    {
                        bulkInsert.BatchSize = 1000;
                        bulkInsert.DestinationTableName = GetTableName();

                        var table = new DataTable();
                        var props = TypeDescriptor.GetProperties(typeof(T))

                            //Dirty hack to make sure we only have system data types
                            //i.e. filter out the relationships/collections
                                                   .Cast<PropertyDescriptor>()

                            // ReSharper disable PossibleNullReferenceException
                                                   .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))

                            // ReSharper restore PossibleNullReferenceException
                                                   .ToArray();

                        foreach (var propertyInfo in props)
                        {
                            bulkInsert.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                            table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                        }

                        var values = new object[props.Length];
                        foreach (var item in items)
                        {
                            for (var i = 0; i < values.Length; i++)
                            {
                                values[i] = props[i].GetValue(item);
                            }

                            table.Rows.Add(values);
                        }

                        if (sqlConnection.State != ConnectionState.Open)
                        {
                            sqlConnection.Open();
                        }

                        bulkInsert.WriteToServer(table);
                    }
                }
                else
                {
                    try
                    {
                        _context.Configuration.AutoDetectChangesEnabled = false;
                        _context.Configuration.ValidateOnSaveEnabled = false;

                        var count = 0;
                        foreach (var item in items)
                        {
                            Entities.Add(item);
                            count++;

                            if (count == 100)
                            {
                                _context.SaveChanges();
                                count = 0;
                            }
                        }

                        if (count > 0)
                        {
                            _context.SaveChanges();
                        }
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        var msg = dbEx.EntityValidationErrors
                            .SelectMany(validationErrors => validationErrors.ValidationErrors)
                            .Aggregate(string.Empty, (current, validationError) => current + (string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + System.Environment.NewLine));

                        throw new Exception(msg, dbEx);
                    }
                }
            }
            catch
            {
                foreach (var entity in items)
                {
                    Entities.Add(entity);
                }
                _context.SaveChanges();
            }
        }

        public void Update(T entity, params Expression<Func<T, dynamic>>[] includePaths)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                var isAttached = false;

                if (_context.Entry(entity).State == EntityState.Detached)
                {
                    // Try attach into context
                    var hashCode = entity.GetHashCode();
                    foreach (var obj in Entities.Local)
                    {
                        if (obj.GetHashCode() == hashCode)
                        {
                            _context.Entry(obj).CurrentValues.SetValues(entity);
                            isAttached = true;
                            break;
                        }
                    }

                    if (!isAttached)
                    {
                        entity = Entities.Attach(entity);
                        _context.Entry(entity).State = EntityState.Modified;
                    }
                }
                else
                {
                    // Set the entity's state to modified
                    _context.Entry(entity).State = EntityState.Modified;
                }

                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = dbEx.EntityValidationErrors
                    .SelectMany(validationErrors => validationErrors.ValidationErrors)
                    .Aggregate(string.Empty, (current, validationError) => current + (System.Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage)));

                throw new Exception(msg, dbEx);
            }
        }

        public void UpdateIncludePaths(T entity, params Expression<Func<T, dynamic>>[] includePaths)
        {
            Update(entity);
        }

        public virtual void UpdateMany(IEnumerable<T> items)
        {
            try
            {
                if (items == null)
                    throw new ArgumentNullException("items");

                foreach (var entity in items)
                {
                    if (_context.Entry(entity).State == EntityState.Detached)
                    {
                        Entities.Attach(entity);
                    }

                    // Set the entity's state to modified
                    _context.Entry(entity).State = EntityState.Modified;
                }
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = dbEx.EntityValidationErrors
                    .SelectMany(validationErrors => validationErrors.ValidationErrors)
                    .Aggregate(string.Empty, (current, validationError) => current + (System.Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage)));

                throw new Exception(msg, dbEx);
            }
        }

        public virtual void UpdateMany(Expression<Func<T, bool>> filterExpression, Expression<Func<T, T>> updateExpression)
        {
            //Entities.Where(filterExpression).Update(updateExpression);
        }

        public IEnumerable<T> ExecuteStoredProcedure(string commandText, params object[] parameters)
        {
            //return _context.ExecuteStoredProcedureList<T>(commandText, parameters);
            return null;
        }

        public void ExecuteStoredProcedure(string commandText, IDictionary<string, object> parameters)
        {
            var command = Context.Database.Connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = commandText;

            if (parameters != null)
            {
                foreach (var item in parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = item.Key;
                    parameter.Value = item.Value;
                    command.Parameters.Add(parameter);
                }
            }

            if (Context.Database.Connection.State != ConnectionState.Open)
            {
                Context.Database.Connection.Open();
            }

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Retrieves the <see cref="StructuralType"/> corresponding to the given CLR type (where the
        /// type is an entity or complex type).
        /// </summary>
        /// <remarks>
        /// If no mapping exists for <paramref name="clrType"/>, but one does exist for one of its base
        /// types, we will return the mapping for the base type.
        /// </remarks>
        /// <param name="workspace">The <see cref="MetadataWorkspace"/></param>
        /// <param name="clrType">The CLR type</param>
        /// <returns>The <see cref="StructuralType"/> corresponding to that CLR type, or <c>null</c> if the Type
        /// is not mapped.</returns>
        private static StructuralType GetEdmType(MetadataWorkspace workspace, Type clrType)
        {
            if (workspace == null)
            {
                throw new ArgumentNullException("workspace");
            }

            if (clrType == null)
            {
                throw new ArgumentNullException("clrType");
            }

            if (clrType.IsPrimitive || clrType == typeof(object))
            {
                // want to avoid loading searching system assemblies for
                // types we know aren't entity or complex types
                return null;
            }

            // We first locate the EdmType in "OSpace", which matches the name and namespace of the CLR type
            EdmType edmType;
            do
            {
                if (!workspace.TryGetType(clrType.Name, clrType.Namespace, DataSpace.OSpace, out edmType))
                {
                    // If EF could not find this type, it could be because it is not loaded into
                    // its current workspace.  In this case, we explicitly load the assembly containing
                    // the CLR type and try again.
                    workspace.LoadFromAssembly(clrType.Assembly);
                    workspace.TryGetType(clrType.Name, clrType.Namespace, DataSpace.OSpace, out edmType);
                }
            }
            while (edmType == null && (clrType = clrType.BaseType) != typeof(object) && clrType != null);

            // Next we locate the StructuralType from the EdmType.
            // This 2-step process is necessary when the types CLR namespace does not match Edm namespace.
            // Look at the EdmEntityTypeAttribute on the generated entity classes to see this Edm namespace.
            StructuralType structuralType = null;
            if (edmType != null &&
                (edmType.BuiltInTypeKind == BuiltInTypeKind.EntityType || edmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType))
            {
                workspace.TryGetEdmSpaceType((StructuralType)edmType, out structuralType);
            }

            return structuralType;
        }

        private string GetTableName()
        {
            var set = _context.Set<T>();
            var regex = new Regex("FROM (?<table>.*) AS");
            var sql = set.ToString();
            var match = regex.Match(sql);

            return match.Groups["table"].Value;
        }
    }
}
