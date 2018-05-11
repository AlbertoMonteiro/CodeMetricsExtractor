Code Metrics Extractor
====================
[![Analytics](https://ga-beacon.appspot.com/UA-63314381-2/CodeMetricsExtractor/README)](https://github.com/AlbertoMonteiro/CodeMetricsExtractor)

With Code Metrics Extractor you can see project health overview, classes health overview and bad methods.


## Project health overview

![Project Health](https://github.com/AlbertoMonteiro/CodeMetricsExtractor/blob/master/Etc/Project%20Health.png?raw=true)

## Classes health overview

![Classes Health](https://github.com/AlbertoMonteiro/CodeMetricsExtractor/blob/master/Etc/Classes%20Health.png?raw=true)

## Bad methods

![Bad methods](https://github.com/AlbertoMonteiro/CodeMetricsExtractor/blob/master/Etc/Bad%20Methods.png?raw=true)

Using project
-------------------
You must install

[Microsoft Build Tools 2015 RC](http://www.microsoft.com/en-us/download/details.aspx?id=46882&WT.mc_id=rss_alldownloads_all)


````
CodeMetrics Extractor.

    Usage:
      MetricsExtractor.exe (-s | --solution) <solution>
      MetricsExtractor.exe -s <solution> [-ip <ignoredProjects>]
      MetricsExtractor.exe -s <solution> [-in <ignoredNamespaces>]
      MetricsExtractor.exe -s <solution> [-it <ignoredTypes>]
      MetricsExtractor.exe -s <solution> [-ip <ignoredProjects>] [-in <ignoredNamespaces>] [-it <ignoredTypes>]
      MetricsExtractor.exe -s <solution> [-jsonconfig <jsonfileconfig>]

    Options:
      -s --solution                                                     Load projects from solution.
      -ip <ignoredProjects> --ignoredprojects <ignoredProjects>         Projets in solution that you want to ignore, split them by "";""
      -in <ignoredNamespaces> --ignorednamespaces <ignoredNamespaces>   Namespaces in your application that you want to ignore, split them by "";""
      -it <ignoredTypes> --ignoredtypes <ignoredTypes>                  Types in your application that you want to ignore, split them by "";""
      -jsonconfig <jsonfileconfig>                                      User a json file to configure metrics extraction


````
