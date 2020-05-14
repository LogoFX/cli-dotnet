Feature: Model Generation
	In order to provide tools to developers who want to add model layer objects
	As an app developer
	I want to be able to install and use the correspondent template via existing dotnet means

Scenario Outline: Generate model entity
	When I install the template pack 'LogoFX.Templates' from local package
	And I create a folder named 'Generation'
	And I generate the code in folder named 'Generation' using 'logofx-model' template with the following options
	| Name                 | Value  |
	| -n                   | Sample |
	| <solutionNameOption> | Test   |
	Then The folder 'Generation' contains generated model entity objects for solution name 'Test'

	Examples:
	| solutionNameOption |
	| -s                 |
	| --solutionName     |

