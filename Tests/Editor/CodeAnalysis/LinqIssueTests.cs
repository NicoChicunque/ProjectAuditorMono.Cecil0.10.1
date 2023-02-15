using System;
using System.Linq;
using NUnit.Framework;
using Unity.ProjectAuditor.Editor;
using Unity.ProjectAuditor.Editor.CodeAnalysis;
using Unity.ProjectAuditor.Editor.Diagnostic;
using Unity.ProjectAuditor.Editor.TestUtils;
using Unity.ProjectAuditor.Editor.Utils;

namespace Unity.ProjectAuditor.EditorTests
{
    class LinqIssueTests : TestFixtureBase
    {
        TestAsset m_TestAsset;

        [SetUp]
        public void SetUp()
        {
            m_TestAsset = new TestAsset("MyClass.cs", @"
using System.Linq;
using System.Collections.Generic;

class MyClass
{
    int Dummy(List<int> list)
    {
        return list.Count();
    }
}"
            );
        }

        [Test]
        public void CodeAnalysis_LinqIssue_IsReported()
        {
            var issues = AnalyzeAndFindAssetIssues(m_TestAsset);

            Assert.AreEqual(1, issues.Count());

            var myIssue = issues.FirstOrDefault();

            Assert.NotNull(myIssue);
            Assert.NotNull(myIssue.descriptor);

            Assert.AreEqual(Severity.Moderate, myIssue.descriptor.defaultSeverity);
            Assert.AreEqual("PAC1000", myIssue.descriptor.id);
            Assert.AreEqual("System.Linq", myIssue.descriptor.type);
            Assert.AreEqual("*", myIssue.descriptor.method);

            Assert.AreEqual(m_TestAsset.fileName, myIssue.filename);
            Assert.AreEqual("'System.Linq.Enumerable.Count' usage", myIssue.description, "Description: {0}", myIssue.description);
            Assert.AreEqual("System.Int32 MyClass::Dummy(System.Collections.Generic.List`1<System.Int32>)", myIssue.GetContext());
            Assert.AreEqual(9, myIssue.line);
            Assert.AreEqual(IssueCategory.Code, myIssue.category);
        }
    }
}
