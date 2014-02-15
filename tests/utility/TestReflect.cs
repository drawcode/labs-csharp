using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Drove.Tests {

    public class TestReflectObject {
        public Guid Id;
        public string Code;
    }

    [TestClass]
    public class TestRelect {
        public TestRelect() {

        }

        private TestContext testContextInstance;

        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void Initialize(TestContext testContext) {

        }

        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void Cleanup() {

        }

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void TestInitialize() {

        }

        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void TestCleanup() {

        }

        [TestMethod]
        public void TestReflect_GetProperties() {

            TestReflectObject obj = new TestReflectObject();
            obj.Code = "Hello";

            PropertyInfo[] props = Reflect.GetProperties(obj);

            Assert.IsNotNull(props);

            Assert.IsTrue(props.Length > 0);                        
        }

        [TestMethod]
        public void TestReflect_CopyTo() {

            TestReflectObject obj = new TestReflectObject();
            obj.Code = "Hello";

            TestReflectObject obj2 = new TestReflectObject();
            obj2.Code = "Hello Again";

            obj2 = obj.CopyTo<TestReflectObject>(obj2);

            Assert.AreEqual<string>(obj.Code, obj2.Code);

            Assert.AreEqual<Guid>(obj.Id, obj2.Id);

        }

        [TestMethod]
        public void TestReflect_Clone() {

            TestReflectObject obj = new TestReflectObject();
            obj.Code = "Hello";

            TestReflectObject obj2 = new TestReflectObject();
            obj2.Code = "Hello Again";

            obj2 = Reflect.Clone<TestReflectObject>(obj, obj2);

            Assert.AreEqual<string>(obj.Code, obj2.Code);

            Assert.AreEqual<Guid>(obj.Id, obj2.Id);
        }


        [TestMethod]
        public void TestReflect_CopyToRef() {

            TestReflectObject obj = new TestReflectObject();
            obj.Code = "Hello";

            TestReflectObject obj2 = new TestReflectObject();
            obj2.Code = "Hello Again";

            obj.CopyTo<TestReflectObject>(ref obj2);

            Assert.AreEqual<string>(obj.Code, obj2.Code);

            Assert.AreEqual<Guid>(obj.Id, obj2.Id);

        }

        [TestMethod]
        public void TestReflect_CloneRef() {

            TestReflectObject obj = new TestReflectObject();
            obj.Code = "Hello";

            TestReflectObject obj2 = new TestReflectObject();
            obj2.Code = "Hello Again";

            Reflect.Clone<TestReflectObject>(obj, ref obj2);

            Assert.AreEqual<string>(obj.Code, obj2.Code);

            Assert.AreEqual<Guid>(obj.Id, obj2.Id);
        }

        [TestMethod]
        public void TestReflect_ObjectSyncFieldValues() {

            TestReflectObject obj = new TestReflectObject();
            obj.Code = "Hello";

            TestReflectObject obj2 = new TestReflectObject();
            obj2.Code = "Hello Again";
            obj2.Status = "obj2 status";

            obj2 = Reflect.ObjectSyncFieldValues<TestReflectObject>(obj, obj2);

            Assert.AreEqual<string>(obj.Code, obj2.Code);

            Assert.AreEqual<Guid>(obj.Id, obj2.Id);

            Assert.AreNotEqual<string>(obj.Status, obj2.Status);
        }

        [TestMethod]
        public void TestReflect_ObjectSyncFieldValuesRef() {

            TestReflectObject obj = new TestReflectObject();
            obj.Code = "Hello";

            TestReflectObject obj2 = new TestReflectObject();
            obj2.Code = "Hello Again";
            obj2.Status = "obj2 status";

            Reflect.ObjectSyncFieldValues<TestReflectObject>(obj, ref obj2);

            Assert.AreEqual<string>(obj.Code, obj2.Code);

            Assert.AreEqual<Guid>(obj.Id, obj2.Id);

            Assert.AreNotEqual<string>(obj.Status, obj2.Status);
        }

        [TestMethod]
        public void TestReflect_ObjectSyncFieldValuesSerialize() {

            TestReflectObject obj = new TestReflectObject();
            obj.Code = "Hello";

            TestReflectObject obj2 = new TestReflectObject();
            obj2.Code = "Hello Again";
            obj2.Status = "obj2 status";

            Reflect.ObjectSyncFieldValuesSerialize<TestReflectObject>(obj, ref obj2);

            Assert.AreEqual<string>(obj.Code, obj2.Code);

            Assert.AreEqual<Guid>(obj.Id, obj2.Id);

            Assert.AreNotEqual<string>(obj.Status, obj2.Status);
        }
        
        [TestMethod]
        public void TestReflect_GetFieldValue() {

            TestReflectObject obj = new TestReflectObject();
            obj.Code = "Hello";

            string code = Reflect.GetFieldValue<string>(obj, "Code");

            Assert.AreEqual<string>(obj.Code, code);
        }

        [TestMethod]
        public void TestReflect_SetFieldValue() {

            TestReflectObject obj = new TestReflectObject();
            obj.Code = "Hello";

            Reflect.SetFieldValue(obj, "Code", "Hello Again");

            Assert.AreEqual<string>(obj.Code, "Hello Again");
        }
    }
}
