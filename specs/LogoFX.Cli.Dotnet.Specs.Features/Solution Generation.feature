Feature: Solution Generation
	In order to provide tools to developers who want to scaffold LogoFX-based solutions
	As a framework developer
	I want to be able to install and use the correspondent template via existing dotnet means

Scenario: Install template
	When I install the 'LogoFX.Templates.WPF' template via dotnet Cli
	Then The template for 'logofx-wpf' is installed with the following parameters
	| Description            | Short Name | Languages | Tags       |
	| LogoFX WPF Application | logofx-wpf | [C#]      | LogoFX/WPF |

Scenario: Generate solution skeleton
	When I install the 'LogoFX.Templates.WPF' template via dotnet Cli
	And I create a folder named 'Generation'
	And I navigate to the folder named 'Generation'
	And I generate the code using 'logofx-wpf' template with the default options
	Then The folder 'Generation' contains working LogoFX template-based solution
