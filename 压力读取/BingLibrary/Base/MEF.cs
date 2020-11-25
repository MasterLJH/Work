using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace BingLibrary.hjb.mvvm
{
    public partial class MEF
    {
        public static object lookup<T>(IEnumerable<Lazy<T, IDictionary<string, object>>> imports, object key, Action<object> defaultValue)
        {
            string keyStr = (key == null) ? "" : key.ToString();
            List<object> finds = new List<object>();
            foreach (var import in imports)
            {
                if (import != null && import.Metadata.ContainsKey(MEF.Key) && import.Metadata[MEF.Key].ToString() == key.ToString())
                {
                    finds.Add(import.Value);
                }
            }

            if (finds.Count == 0)
            {
                if (keyStr != null)
                    Debug.WriteLine("■■■未找到键：" + keyStr + "。已采用默认值。");
                return defaultValue;
            }
            else if (finds.Count == 1)
            {
                return finds[0];
            }
            else
            {
                Debug.WriteLine("■■■找到重复键：" + keyStr + "。已采用默认值。");
                return defaultValue;
            }
        }

        public static CompositionContainer container()
        {
            var fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
            var catalog = new DirectoryCatalog(fi.DirectoryName);
            return new CompositionContainer(catalog);
        }

        public static T compose<T>(T part)
        {
            container().ComposeParts(part);
            return part;
        }
    }

    ////遗忘系列-初代
    //public static class TestMEFContracts
    //{
    //    public const string TestContract1 = "1234567";
    //}

    //public class TestModel
    //{
    //    public string testString { set; get; } = "";

    //    [Export(TestMEFContracts.TestContract1)]
    //    public string TestFunc1(string s)
    //    {
    //        testString = s;
    //        return s;
    //    }
    //}

    //[Export(MEF.Contracts.Data)]
    //[ExportMetadata(MEF.Key, "md1")]
    //public class TestViewModel : DataSourceOriginal
    //{
    //    public class ImportTest
    //    {
    //        [Import(TestMEFContracts.TestContract1)]
    //        public Func<string, string> testFunc1 { set; get; }
    //    }

    //    public ImportTest it = (ImportTest)MEF.compose(new ImportTest());

    //    //[Export(MEF.Contracts.Execute)]
    //    //[ExportMetadata(MEF.Key, "TestForgetRun")]
    //    public void TestForgetRun()
    //    {
    //        it.testFunc1("Hello World");
    //    }
    //}
}