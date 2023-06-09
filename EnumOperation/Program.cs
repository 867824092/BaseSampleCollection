using System.ComponentModel;

public class Program {
    static void Main(string[] args) {
       
     
        {
            Console.WriteLine((int)(FileAttributes.ReadOnly | FileAttributes.System | FileAttributes.Directory));
            BlogFile blogFile = new BlogFile("C#", FileAttributes.Normal | FileAttributes.ReadOnly);
            Console.WriteLine(Convert.ToString((int)FileAttributes.ReadOnly,2));
            Console.WriteLine( Convert.ToString((int)FileAttributes.Normal,2));
        }

        {
            Console.WriteLine((int)(EnumReportContentType.Daily | EnumReportContentType.Weekly | EnumReportContentType.Monthly 
                                    | EnumReportContentType.Year));
            Console.WriteLine((int)(EnumReportContentType.Daily | EnumReportContentType.Weekly | EnumReportContentType.Monthly));
            Console.WriteLine((int)(EnumReportContentType.Daily | EnumReportContentType.Weekly));
            
            Console.WriteLine("月报类型是否选中：{0}",(int)(EnumReportContentType.Monthly & (EnumReportContentType.Daily | EnumReportContentType.Weekly)));
            Console.WriteLine("日报类型是否选中：{0}",(int)(EnumReportContentType.Daily & (EnumReportContentType.Monthly | EnumReportContentType.Weekly)));
            Console.WriteLine("日报类型是否选中：{0}",(int)(EnumReportContentType.Daily & (EnumReportContentType.Daily | EnumReportContentType.Weekly)));
            Console.WriteLine("年报类型是否选中：{0}",(int)(EnumReportContentType.Year & (EnumReportContentType.Daily | EnumReportContentType.Weekly| EnumReportContentType.Monthly)));
            Console.WriteLine("周报类型是否选中：{0}",(int)(EnumReportContentType.Weekly & (EnumReportContentType.Monthly | EnumReportContentType.Weekly)));
            //1011 
            //1000
        }
        {
            List<ReportEmails> reportFiles = new List<ReportEmails>
            {
            new ReportEmails("1@qq.com",EnumReportContentType.Daily | EnumReportContentType.Weekly),
            new ReportEmails("2@qq.com",EnumReportContentType.Daily | EnumReportContentType.Monthly),
            new ReportEmails("3@qq.com",EnumReportContentType.Year | EnumReportContentType.Monthly),
            new ReportEmails("4@qq.com",EnumReportContentType.Weekly | EnumReportContentType.Monthly)
            };

            foreach (var reportEmails in reportFiles.Where(u => (u.ReportContentType & EnumReportContentType.Daily) > 0)) {
                Console.WriteLine(reportEmails.Email,reportEmails.ReportContentType);
            }
            
            foreach (var reportEmails in reportFiles.Where(u => (u.ReportContentType & EnumReportContentType.Monthly) > 0)) {
                Console.WriteLine(reportEmails.Email,reportEmails.ReportContentType);
            }
        }
    }
}

public class BlogFile {
    public string Name { get; set; }
    public  FileAttributes FileAttributes { get; set; }

    public BlogFile(string name,FileAttributes fileAttributes = FileAttributes.Normal) {
        Name = name;
        FileAttributes = fileAttributes;
    }
}

public class ReportEmails {
    public EnumReportContentType ReportContentType { get; init; }
    public string Email { get; init; }

    public ReportEmails(string email,EnumReportContentType reportContentType) {
        Email = email;
        ReportContentType = reportContentType;
    }
}

/// <summary>
/// 推送报表类型
/// </summary>
[Flags]
public enum EnumReportContentType
{
    /// <summary>
    /// 日报
    /// </summary>
    Daily = 1,
    /// <summary>
    /// 周报
    /// </summary>
    Weekly = 2,
    /// <summary>
    /// 月报
    /// </summary>
    Monthly = 4,
    /// <summary>
    /// 年报
    /// </summary>
    Year = 8
}