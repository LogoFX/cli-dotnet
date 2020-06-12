Feature: Solution Generation
	In order to provide tools to developers who want to scaffold LogoFX-based solutions
	As a framework developer
	I want to be able to install and use the correspondent template via existing dotnet means

Scenario: Generate solution skeleton
	When I install the template pack 'LogoFX.Templates' from local package
	And I create a folder named 'Generation'
	And I generate the code in folder named 'Generation' using 'logofx-wpf' template with the default options
	Then The folder 'Generation' contains working LogoFX template-based solution
