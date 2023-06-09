namespace LocalizationDemo; 

public class TableStructure {
    public TableStructure() {
        Columns = new List<ColumnDescription>();
    }

    public List<ColumnDescription> Columns { get; set; }
    public string NameSpace { get; set; }
}
    
    
public sealed class ColumnDescription {
    public string ColumnName { get; set; }
    public string ColumnType { get; set; }
    public string Comment { get; set; }
    public bool IsCanNull { get; set; }
}