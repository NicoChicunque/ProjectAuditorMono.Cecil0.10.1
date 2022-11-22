using System;
using System.Collections;
using NUnit.Framework;
using Unity.ProjectAuditor.Editor;
using Unity.ProjectAuditor.Editor.CodeAnalysis;
using Unity.ProjectAuditor.Editor.Core;
using Unity.ProjectAuditor.Editor.Diagnostic;
using Unity.ProjectAuditor.Editor.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Unity.ProjectAuditor.EditorTests
{
    [Serializable]
    class ProjectIssueTests
    {
        Descriptor m_Descriptor = new Descriptor
            (
            "TD2001",
            "test",
            Area.CPU,
            "this is not actually a problem",
            "do nothing"
            );

        Descriptor m_CriticalIssueDescriptor = new Descriptor
            (
            "TD2002",
            "test",
            Area.CPU,
            "this is not actually a problem",
            "do nothing"
            )
        {
            critical = true
        };

        [SerializeField]
        ProjectIssue m_Issue;

        [Test]
        public void ProjectIssue_NewIssue_IsInitialized()
        {
            var description = "dummy issue";
            var uninitialised = new ProjectIssue(IssueCategory.Code, m_Descriptor, description);
            Assert.AreEqual(string.Empty, uninitialised.filename);
            Assert.AreEqual(string.Empty, uninitialised.relativePath);
            Assert.AreEqual(string.Empty, uninitialised.GetContext());
            Assert.AreEqual(description, uninitialised.description);
            Assert.IsFalse(uninitialised.isCritical);
        }

        [Test]
        public void ProjectIssue_NewIssue_IsCritical()
        {
            var description = "dummy issue";
            var diagnostic = new ProjectIssue(IssueCategory.Code, m_CriticalIssueDescriptor, description);
            Assert.AreEqual(string.Empty, diagnostic.filename);
            Assert.AreEqual(string.Empty, diagnostic.relativePath);
            Assert.AreEqual(string.Empty, diagnostic.GetContext());
            Assert.AreEqual(description, diagnostic.description);

            // the issue should be critical as per the descriptor
            Assert.IsTrue(diagnostic.isCritical);
        }

        [UnityTest]
        public IEnumerator ProjectIssue_Critical_PersistsAfterDomainReload()
        {
            var description = "dummy issue";
            m_Issue = new ProjectIssue(IssueCategory.Code, m_Descriptor, description);

            Assert.IsFalse(m_Issue.isCritical);

            m_Issue.isCritical = true;

            Assert.IsTrue(m_Issue.isCritical);
#if UNITY_2019_3_OR_NEWER
            EditorUtility.RequestScriptReload();
            yield return new WaitForDomainReload();

            Assert.IsTrue(m_Issue.isCritical);
#else
            yield return null;
#endif
        }

        [Test]
        public void ProjectIssue_CustomProperties_AreSet()
        {
            object[] properties =
            {
                "property #0",
                "property #1"
            };
            ProjectIssue issue = ProjectIssue.Create(IssueCategory.Code, m_Descriptor, "dummy issue")
                .WithCustomProperties(properties);

            Assert.AreEqual(2, issue.GetNumCustomProperties());
            Assert.AreEqual(properties[0], issue.GetCustomProperty(0));
            Assert.AreEqual(properties[1], issue.GetCustomProperty(1));
        }

        [Test]
        public void ProjectIssue_CustomProperties_AreNotSet()
        {
            var issue = new ProjectIssue(IssueCategory.Code, m_Descriptor, "dummy issue");

            Assert.AreEqual(0, issue.GetNumCustomProperties());
        }

        [Test]
        public void ProjectIssue_Properties_AreSet()
        {
            object[] properties =
            {
                "property #0",
                "property #1"
            };
            ProjectIssue issue = ProjectIssue.Create(IssueCategory.Code, m_Descriptor, "dummy issue")
                .WithCustomProperties(properties)
                .WithLocation("Assets/Dummy.cs");

            Assert.AreEqual(2, issue.GetNumCustomProperties());
            Assert.AreEqual("dummy issue", issue.GetProperty(PropertyType.Description));

            Assert.AreEqual(Severity.Default.ToString(), issue.GetProperty(PropertyType.Severity));
            Assert.AreEqual(Area.CPU.ToString(), issue.GetProperty(PropertyType.Area));
            Assert.AreEqual("Assets/Dummy.cs:0", issue.GetProperty(PropertyType.Path));
            Assert.AreEqual("Dummy.cs:0", issue.GetProperty(PropertyType.Filename));
            Assert.AreEqual("cs", issue.GetProperty(PropertyType.FileType));
            Assert.AreEqual(false.ToString(), issue.GetProperty(PropertyType.CriticalContext));
            Assert.AreEqual(properties[0], issue.GetProperty(PropertyType.Num));
            Assert.AreEqual(properties[1], issue.GetProperty(PropertyType.Num + 1));
        }

        [Test]
        public void ProjectIssue_NoFileProperties_AreSet()
        {
            var issue = new ProjectIssue(IssueCategory.Code, m_Descriptor, "dummy issue");

            Assert.AreEqual(ProjectIssueExtensions.k_NotAvailable, issue.GetProperty(PropertyType.Path));
            Assert.AreEqual(ProjectIssueExtensions.k_NotAvailable, issue.GetProperty(PropertyType.Filename));
            Assert.AreEqual(ProjectIssueExtensions.k_NotAvailable, issue.GetProperty(PropertyType.FileType));
        }
    }
}
