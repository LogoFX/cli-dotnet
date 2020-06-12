Feature: Template Pack Setup
	In order to write LogoFX-based code in a fast and efficient way
	As an app developer
	I want to be able to add new LogoFX-based elements via cli

Scenario: Install template pack
	When I install the template pack 'LogoFX.Templates' from local package
	Then The template for 'logofx-wpf' is installed with the following parameters
	| Description            | Short Name | Languages | Tags       |
	| LogoFX WPF Application | logofx-wpf | [C#]      | LogoFX/WPF |
	And The template for 'logofx-entity' is installed with the following parameters
	| Description         | Short Name    | Languages | Tags          |
	| LogoFX Model Entity | logofx-entity | [C#]      | LogoFX/Entity |
	And The template for 'logofx-service' is installed with the following parameters
	| Description          | Short Name     | Languages | Tags           |
	| LogoFX Model Service | logofx-service | [C#]      | LogoFX/Service |
