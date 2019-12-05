﻿using System.Collections;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Unity.ProjectAuditor.Editor;
using UnityEngine;

namespace UnityEditor.ProjectAuditor.EditorTests
{
	class ScriptIssueTest : ScriptIssueTestBase {
			
		[SetUp]
		public void SetUp()
		{
			CreateScript("using UnityEngine; class MyClass : MonoBehaviour { void Start() { Debug.Log(Camera.main.name); } }");
		}

		[TearDown]
		public void TearDown()
		{
			DeleteScript();
		}

		[Test]
		public void AnalysisTestPasses()
		{
			var projectReport = new ProjectReport();
			var projectAuditor = new Unity.ProjectAuditor.Editor.ProjectAuditor();

			projectAuditor.Audit(projectReport);
			var issues = projectReport.GetIssues(IssueCategory.ApiCalls);

			Assert.NotNull(issues);
			
			Assert.Positive(issues.Count());

			issues = issues.Where(i => i.relativePath.Equals(relativePath));
			
			Assert.Positive(issues.Count());
			
			var myIssue = issues.FirstOrDefault();
			
			Assert.NotNull(myIssue);
			Assert.NotNull(myIssue.descriptor);
			
			Assert.AreEqual(Rule.Action.Default, myIssue.descriptor.action);
			Assert.AreEqual(101000, myIssue.descriptor.id);
			Assert.True(myIssue.descriptor.type.Equals("UnityEngine.Camera"));
			Assert.True(myIssue.descriptor.method.Equals("main"));
			
			Assert.True(myIssue.name.Equals("Camera.get_main"));
			Assert.True(myIssue.filename.Equals(m_ScriptName));
			Assert.True(myIssue.description.Equals("UnityEngine.Camera.main"));
			Assert.True(myIssue.callingMethod.Equals("System.Void MyClass::Start()"));
			Assert.AreEqual(1, myIssue.line);
			Assert.AreEqual(IssueCategory.ApiCalls, myIssue.category);
		}
	}	
}

