using System;
using System.Threading;
using NUnit.Framework;

using Konata.Resource.Localization;

namespace Konata.Test.Simple
{
    /// <summary>
    /// 测试用例标准模板参考
    /// </summary>
    [TestFixture(Description = "测试模板")]
    public class SimpleTest : BaseTest
    {
        [SetUp]
        public void Setup()
        {
            Console.WriteLine($"开始进行功能测试");
        }

        [Test(Description = "test1")]
        [Category("多语言测试")]
        public void Test1()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            Console.WriteLine(Localization.TestString);
            Assert.AreEqual(Localization.TestString, "TestString");
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-CN");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-CN");
            Console.WriteLine(Localization.TestString);
            Assert.AreEqual(Localization.TestString, "测试字符串");
        }

        [Test(Description ="test2")]
        [Category("反射获取方法参数")]
        public void Test2()
        {

        }

        [TearDown]
        public void Dispose()
        {
            Console.WriteLine("释放资源");
        }
    }
}