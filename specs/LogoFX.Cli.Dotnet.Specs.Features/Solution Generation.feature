Feature: Solution Generation
	In order to provide tools to developers who want to scaffold LogoFX-based solutions
	As a framework developer
	I want to be able to install and use the correspondent template via existing dotnet means

@ignore
Scenario: Install template pack
	When I install the template pack 'LogoFX.Templates' from local package
	Then The template for 'logofx-wpf' is installed with the following parameters
	| Description            | Short Name | Languages | Tags       |
	| LogoFX WPF Application | logofx-wpf | [C#]      | LogoFX/WPF |
	And The template for 'logofx-model' is installed with the following parameters
	| Description         | Short Name   | Languages | Tags         |
	| LogoFX Model Entity | logofx-model | [C#]      | LogoFX/Model |

@ignore
Scenario: Generate solution skeleton
	When I install the template pack 'LogoFX.Templates' from local package
	And I create a folder named 'Generation'
	And I generate the code in folder named 'Generation' using 'logofx-wpf' template with the default options
	Then The folder 'Generation' contains working LogoFX template-based solution
