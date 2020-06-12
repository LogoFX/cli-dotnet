Feature: Data Service Generation
	In order to provide tools to developers who want to add data service layer objects
	As an app developer
	I want to be able to install and use the correspondent template via existing dotnet means


Scenario Outline: Generate entity service in an empty folder
	When I install the template pack 'LogoFX.Templates' from local package
	And I create a folder named 'Generation'
	And I generate the code in folder named 'Generation' using 'logofx-service' template with the following options
	| Name                 | Value             |
	| -n                   | <entityNameValue> |
	| <solutionNameOption> | Test              |
	| --allow-scripts      | yes               |
	Then The folder 'Generation' contains generated entity objects for name '<entityNameValue>' for solution name 'Test'
	And The folder 'Generation' contains generated entity service objects for name '<entityNameValue>' for solution name 'Test'

	Examples:
	| entityNameValue | solutionNameOption |
	| Sample          | -sn                |
	| Another         | --solution-name    |

Scenario Outline: Generate entity service in existing solution
	When I install the template pack 'LogoFX.Templates' from local package
	And I create a folder named 'Generation'
	And I generate the code in folder named 'Generation' using 'logofx-wpf' template with the default options
	And I generate the code in folder named 'Generation' using 'logofx-service' template with the following options
	| Name                 | Value  |
	| -n                   | Sample |
	| <solutionNameOption> | Test   |
	| --allow-scripts      | yes    |
	Then The folder 'Generation' contains working LogoFX template-based solution
	And The folder 'Generation' contains generated entity objects for name 'Sample' for solution name 'Test'
	And The folder 'Generation' contains generated entity service objects for name 'Sample' for solution name 'Test'

	Examples:
	| solutionNameOption |
	| -sn                |
	| --solution-name    |