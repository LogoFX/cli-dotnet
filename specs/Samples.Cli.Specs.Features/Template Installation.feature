Feature: Template Installation
	In order to provide template to developers who want to scaffold LogoFX-based solutions
	As a framework developer
	I want to be able to install the template via existing dotnet means

Scenario: Install template
	When I install the basics template via dotnet Cli
	Then The template for 'LogoFX' is installed with the following parameters
	| Templates     | Short Name    | Language | Tags       |
	| LogoFX Basics | logofx-basics | C#       | LogoFX;WPF |
