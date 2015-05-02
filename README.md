Code Metrics Extractor
====================

With Code Metrics Extractor you can see project health overview, classes health overview and bad methods.


## Project health overview

![Project Health](/Etc/Project Health.png)

## Classes health overview

![Classes Health](/Etc/Classes Health.png)

## Bad methods

![Bad methods](/Etc/Bad Methods.png)

Using project
-------------------
You must install

[Microsoft Build Tools 2015 RC](http://www.microsoft.com/en-us/download/details.aspx?id=46882&WT.mc_id=rss_alldownloads_all)


````
MetricsExtractor.exe -solution SolutionPath.sln
````

Aditional parameters:

#### IgnoredProjects 
You can list projets in solution that you want to ignore, you must split them by "**;**"

Example:

````
metricsextractor.exe -solution solutionpath.sln -ignoredprojects "Project.Core.Tests;Project.Data.Tests;Project.Web.Tests"
````
