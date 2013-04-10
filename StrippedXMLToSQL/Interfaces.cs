using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.IO;
using System.Linq;
using System.Text;

namespace StrippedXMLToSQL
{
    public interface IRepositoryDataContext
    {
        System.Data.Linq.Table<TEntity> GetTable<TEntity>() where TEntity : class;
    }

    public interface ICommField
    {
        int TableID { get; set; }
        string FieldName { get; set; }
        int FieldType { get; set; }

    }
    public interface ICommTable
    {
        int TableID { get; set; }
        string TableName { get; set; }
    }
    public interface IRepositoryFactory
    {
        Stream CreateXmlMapReader();
        IRepositoryDataContext CreateAdapter(XmlMappingSource objSource);
        IEnumerable<ICommField> CreateCommFieldEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId);
        IEnumerable<ICommTable> CreateCommTableEnumeration(IRepositoryDataContext repositoryDataContext, int intTableId);
    }
}
