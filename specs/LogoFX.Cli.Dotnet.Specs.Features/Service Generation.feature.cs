﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.5.0.0
//      SpecFlow Generator Version:3.5.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace LogoFX.Cli.Dotnet.Specs.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.5.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class DataServiceGenerationFeature : object, Xunit.IClassFixture<DataServiceGenerationFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "Service Generation.feature"
#line hidden
        
        public DataServiceGenerationFeature(DataServiceGenerationFeature.FixtureData fixtureData, LogoFX_Cli_Dotnet_Specs_Features_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "", "Data Service Generation", "\tIn order to provide tools to developers who want to add data service layer objec" +
                    "ts\n\tAs an app developer\n\tI want to be able to install and use the correspondent " +
                    "template via existing dotnet means", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Generate entity service in an empty folder")]
        [Xunit.TraitAttribute("FeatureTitle", "Data Service Generation")]
        [Xunit.TraitAttribute("Description", "Generate entity service in an empty folder")]
        [Xunit.InlineDataAttribute("Sample", "-sn", new string[0])]
        [Xunit.InlineDataAttribute("Another", "--solution-name", new string[0])]
        public virtual void GenerateEntityServiceInAnEmptyFolder(string entityNameValue, string solutionNameOption, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("entityNameValue", entityNameValue);
            argumentsOfScenario.Add("solutionNameOption", solutionNameOption);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Generate entity service in an empty folder", null, tagsOfScenario, argumentsOfScenario);
#line 7
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 8
 testRunner.When("I install the template pack \'LogoFX.Templates\' from local package", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 9
 testRunner.And("I create a folder named \'Generation\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                            "Name",
                            "Value"});
                table3.AddRow(new string[] {
                            "-n",
                            string.Format("{0}", entityNameValue)});
                table3.AddRow(new string[] {
                            string.Format("{0}", solutionNameOption),
                            "Test"});
                table3.AddRow(new string[] {
                            "--allow-scripts",
                            "yes"});
#line 10
 testRunner.And("I generate the code in folder named \'Generation\' using \'logofx-service\' template " +
                        "with the following options", ((string)(null)), table3, "And ");
#line hidden
#line 15
 testRunner.Then(string.Format("The folder \'Generation\' contains generated entity objects for name \'{0}\' for solu" +
                            "tion name \'Test\'", entityNameValue), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 16
 testRunner.And(string.Format("The folder \'Generation\' contains generated entity service objects for name \'{0}\' " +
                            "for solution name \'Test\'", entityNameValue), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Generate entity service in existing solution")]
        [Xunit.TraitAttribute("FeatureTitle", "Data Service Generation")]
        [Xunit.TraitAttribute("Description", "Generate entity service in existing solution")]
        [Xunit.InlineDataAttribute("-sn", new string[0])]
        [Xunit.InlineDataAttribute("--solution-name", new string[0])]
        public virtual void GenerateEntityServiceInExistingSolution(string solutionNameOption, string[] exampleTags)
        {
            string[] tagsOfScenario = exampleTags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("solutionNameOption", solutionNameOption);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Generate entity service in existing solution", null, tagsOfScenario, argumentsOfScenario);
#line 23
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 24
 testRunner.When("I install the template pack \'LogoFX.Templates\' from local package", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 25
 testRunner.And("I create a folder named \'Generation\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 26
 testRunner.And("I generate the code in folder named \'Generation\' using \'logofx-wpf\' template with" +
                        " the default options", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
                TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                            "Name",
                            "Value"});
                table4.AddRow(new string[] {
                            "-n",
                            "Sample"});
                table4.AddRow(new string[] {
                            string.Format("{0}", solutionNameOption),
                            "Test"});
                table4.AddRow(new string[] {
                            "--allow-scripts",
                            "yes"});
#line 27
 testRunner.And("I generate the code in folder named \'Generation\' using \'logofx-service\' template " +
                        "with the following options", ((string)(null)), table4, "And ");
#line hidden
#line 32
 testRunner.Then("The folder \'Generation\' contains working LogoFX template-based solution", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 33
 testRunner.And("The folder \'Generation\' contains generated entity objects for name \'Sample\' for s" +
                        "olution name \'Test\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 34
 testRunner.And("The folder \'Generation\' contains generated entity service objects for name \'Sampl" +
                        "e\' for solution name \'Test\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.5.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                DataServiceGenerationFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                DataServiceGenerationFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
