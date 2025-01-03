# Erwine.Leonard.T.Htmltility Powershell Module

Provides utility functions that are useful for parsing and modifying HTML.

## Contributing{#contributed}

This PowerShell module is maintained by Leonard T. Erwine. If you wish to contribute to this project, fork [this repository](https://github.com/lerwine/PowerShell-Modules) and generate a pull request.

- [HTML Agility Pack Documentation](https://html-agility-pack.net/documentation)
  - [NuGet Package](https://www.nuget.org/packages/HtmlAgilityPack)
  - [GitHub Repository](https://github.com/zzzprojects/html-agility-pack)
- [Markdig Documentation](https://github.com/xoofx/markdig/blob/master/readme.md)
- [Pester Assertion Reference](https://pester.dev/docs/assertions)

## Cmdlet Test Cases

### Select-MarkdownObject

#### ExplicitDepth Parameter Set

| Type                                       | Depth | IncludeAttributes | Input Object Processor |
| ------------------------------------------ | ----- | ----------------- | ---------------------- |
| *(not present)*                            | `0`   | *(not present)*   |                        |
| *(not present)*                            | `1`   | *(not present)*   |                        |
| *(not present)*                            | `> 1` | *(not present)*   |                        |
| `Block`                                    | `0`   | *(not present)*   |                        |
| `HtmlAttributes`                           | `0`   | *(not present)*   |                        |
| `Any`                                      | `0`   | *(not present)*   |                        |
| `Block`, `Inline`                          | `0`   | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `0`   | *(not present)*   |                        |
| `Block`, `Any`                             | `0`   | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `0`   | *(not present)*   |                        |
| `Block`, `Inline`                          | `0`   | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `0`   | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `0`   | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `0`   | *(not present)*   |                        |
| `Block`                                    | `1`   | *(not present)*   |                        |
| `HtmlAttributes`                           | `1`   | *(not present)*   |                        |
| `Any`                                      | `1`   | *(not present)*   |                        |
| `Block`, `Inline`                          | `1`   | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `1`   | *(not present)*   |                        |
| `Block`, `Any`                             | `1`   | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | `1`   | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `1`   | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `1`   | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `1`   | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `1`   | *(not present)*   |                        |
| `Block`                                    | `> 1` | *(not present)*   |                        |
| `HtmlAttributes`                           | `> 1` | *(not present)*   |                        |
| `Any`                                      | `> 1` | *(not present)*   |                        |
| `Block`, `Inline`                          | `> 1` | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `> 1` | *(not present)*   |                        |
| `Block`, `Any`                             | `> 1` | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | `> 1` | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `> 1` | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `> 1` | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `> 1` | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `> 1` | *(not present)*   |                        |
| *(not present)*                            | `0`   | Present           |                        |
| *(not present)*                            | `1`   | Present           |                        |
| *(not present)*                            | `> 1` | Present           |                        |
| `Block`                                    | `0`   | Present           |                        |
| `HtmlAttributes`                           | `0`   | Present           |                        |
| `Block`, `Inline`                          | `0`   | Present           |                        |
| `Block`, `Any`                             | `0`   | Present           |                        |
| `Block`, `Inline`, `Any`                   | `0`   | Present           |                        |
| `Block`                                    | `1`   | Present           |                        |
| `HtmlAttributes`                           | `1`   | Present           |                        |
| `Any`                                      | `1`   | Present           |                        |
| `Block`, `Inline`                          | `1`   | Present           |                        |
| `Block`, `Any`                             | `1`   | Present           |                        |
| `Block`, `Inline`, `Any`                   | `1`   | Present           |                        |
| `Block`                                    | `> 1` | Present           |                        |
| `HtmlAttributes`                           | `> 1` | Present           |                        |
| `Any`                                      | `> 1` | Present           |                        |
| `Block`, `Inline`                          | `> 1` | Present           |                        |
| `Block`, `Any`                             | `> 1` | Present           |                        |
| `Block`, `Inline`, `Any`                   | `> 1` | Present           |                        |

#### Recurse Parameter Set

| Type                                       | Recurse | IncludeAttributes | Input Object Processor |
| ------------------------------------------ | ------- | ----------------- | ---------------------- |
| *(not present)*                            | Present | *(not present)*   |                        |
| `Block`                                    | Present | *(not present)*   |                        |
| `HtmlAttributes`                           | Present | *(not present)*   |                        |
| `Any`                                      | Present | *(not present)*   |                        |
| `Block`, `Inline`                          | Present | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | Present | *(not present)*   |                        |
| `Block`, `Any`                             | Present | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | Present | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | Present | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | Present | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | Present | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | Present | *(not present)*   |                        |
| *(not present)*                            | Present | Present           |                        |
| `Block`                                    | Present | Present           |                        |
| `HtmlAttributes`                           | Present | Present           |                        |
| `Any`                                      | Present | Present           |                        |
| `Block`, `Inline`                          | Present | Present           |                        |
| `Block`, `Any`                             | Present | Present           |                        |
| `Block`, `Inline`, `Any`                   | Present | Present           |                        |

#### RecurseUnmatched Parameter Set

| Type                                       | MinDepth        | MaxDepth               | RecurseUnmatchedOnly | Input Object Processor |
| ------------------------------------------ | --------------- | ---------------------- | -------------------- | ---------------------- |
| `Block`                                    | *(not present)* | *(not present)*        | Present              |                        |
| `HtmlAttributes`                           | *(not present)* | *(not present)*        | Present              |                        |
| `Any`                                      | *(not present)* | *(not present)*        | Present              |                        |
| `Block`, `Inline`                          | *(not present)* | *(not present)*        | Present              |                        |
| `Block`, `HtmlAttributes`                  | *(not present)* | *(not present)*        | Present              |                        |
| `Block`, `Any`                             | *(not present)* | *(not present)*        | Present              |                        |
| `HtmlAttributes`, `Any`                    | *(not present)* | *(not present)*        | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | *(not present)* | *(not present)*        | Present              |                        |
| `Block`, `Inline`, `Any`                   | *(not present)* | *(not present)*        | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | *(not present)* | *(not present)*        | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | *(not present)* | *(not present)*        | Present              |                        |
| `Block`                                    | `0`             | *(not present)*        | Present              |                        |
| `HtmlAttributes`                           | `0`             | *(not present)*        | Present              |                        |
| `Any`                                      | `0`             | *(not present)*        | Present              |                        |
| `Block`, `Inline`                          | `0`             | *(not present)*        | Present              |                        |
| `Block`, `HtmlAttributes`                  | `0`             | *(not present)*        | Present              |                        |
| `Block`, `Any`                             | `0`             | *(not present)*        | Present              |                        |
| `HtmlAttributes`, `Any`                    | `0`             | *(not present)*        | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `0`             | *(not present)*        | Present              |                        |
| `Block`, `Inline`, `Any`                   | `0`             | *(not present)*        | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | `0`             | *(not present)*        | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `0`             | *(not present)*        | Present              |                        |
| `Block`                                    | `1`             | *(not present)*        | Present              |                        |
| `HtmlAttributes`                           | `1`             | *(not present)*        | Present              |                        |
| `Block`, `Inline`                          | `1`             | *(not present)*        | Present              |                        |
| `Block`, `HtmlAttributes`                  | `1`             | *(not present)*        | Present              |                        |
| `Block`, `Any`                             | `1`             | *(not present)*        | Present              |                        |
| `HtmlAttributes`, `Any`                    | `1`             | *(not present)*        | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `1`             | *(not present)*        | Present              |                        |
| `Block`, `Inline`, `Any`                   | `1`             | *(not present)*        | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | `1`             | *(not present)*        | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `1`             | *(not present)*        | Present              |                        |
| `Block`                                    | `> 1`           | *(not present)*        | Present              |                        |
| `HtmlAttributes`                           | `> 1`           | *(not present)*        | Present              |                        |
| `Any`                                      | `> 1`           | *(not present)*        | Present              |                        |
| `Block`, `Inline`                          | `> 1`           | *(not present)*        | Present              |                        |
| `Block`, `HtmlAttributes`                  | `> 1`           | *(not present)*        | Present              |                        |
| `Block`, `Any`                             | `> 1`           | *(not present)*        | Present              |                        |
| `HtmlAttributes`, `Any`                    | `> 1`           | *(not present)*        | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `> 1`           | *(not present)*        | Present              |                        |
| `Block`, `Inline`, `Any`                   | `> 1`           | *(not present)*        | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | `> 1`           | *(not present)*        | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `> 1`           | *(not present)*        | Present              |                        |
| `Block`                                    | *(not present)* | `0`                    | Present              |                        |
| `HtmlAttributes`                           | *(not present)* | `0`                    | Present              |                        |
| `Any`                                      | *(not present)* | `0`                    | Present              |                        |
| `Block`, `Inline`                          | *(not present)* | `0`                    | Present              |                        |
| `Block`, `HtmlAttributes`                  | *(not present)* | `0`                    | Present              |                        |
| `Block`, `Any`                             | *(not present)* | `0`                    | Present              |                        |
| `HtmlAttributes`, `Any`                    | *(not present)* | `0`                    | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | *(not present)* | `0`                    | Present              |                        |
| `Block`, `Inline`, `Any`                   | *(not present)* | `0`                    | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | *(not present)* | `0`                    | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | *(not present)* | `0`                    | Present              |                        |
| `Block`                                    | *(not present)* | `1`                    | Present              |                        |
| `HtmlAttributes`                           | *(not present)* | `1`                    | Present              |                        |
| `Any`                                      | *(not present)* | `1`                    | Present              |                        |
| `Block`, `Inline`                          | *(not present)* | `1`                    | Present              |                        |
| `Block`, `HtmlAttributes`                  | *(not present)* | `1`                    | Present              |                        |
| `Block`, `Any`                             | *(not present)* | `1`                    | Present              |                        |
| `HtmlAttributes`, `Any`                    | *(not present)* | `1`                    | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | *(not present)* | `1`                    | Present              |                        |
| `Block`, `Inline`, `Any`                   | *(not present)* | `1`                    | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | *(not present)* | `1`                    | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | *(not present)* | `1`                    | Present              |                        |
| `Block`                                    | *(not present)* | `> 1`                  | Present              |                        |
| `HtmlAttributes`                           | *(not present)* | `> 1`                  | Present              |                        |
| `Any`                                      | *(not present)* | `> 1`                  | Present              |                        |
| `Block`, `Inline`                          | *(not present)* | `> 1`                  | Present              |                        |
| `Block`, `HtmlAttributes`                  | *(not present)* | `> 1`                  | Present              |                        |
| `Block`, `Any`                             | *(not present)* | `> 1`                  | Present              |                        |
| `HtmlAttributes`, `Any`                    | *(not present)* | `> 1`                  | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | *(not present)* | `> 1`                  | Present              |                        |
| `Block`, `Inline`, `Any`                   | *(not present)* | `> 1`                  | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | *(not present)* | `> 1`                  | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | *(not present)* | `> 1`                  | Present              |                        |
| `Block`                                    | `0`             | `0`                    | Present              |                        |
| `HtmlAttributes`                           | `0`             | `0`                    | Present              |                        |
| `Any`                                      | `0`             | `0`                    | Present              |                        |
| `Block`, `Inline`                          | `0`             | `0`                    | Present              |                        |
| `Block`, `HtmlAttributes`                  | `0`             | `0`                    | Present              |                        |
| `Block`, `Any`                             | `0`             | `0`                    | Present              |                        |
| `HtmlAttributes`, `Any`                    | `0`             | `0`                    | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `0`             | `0`                    | Present              |                        |
| `Block`, `Inline`, `Any`                   | `0`             | `0`                    | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | `0`             | `0`                    | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `0`             | `0`                    | Present              |                        |
| `Block`                                    | `0`             | `1`                    | Present              |                        |
| `HtmlAttributes`                           | `0`             | `1`                    | Present              |                        |
| `Any`                                      | `0`             | `1`                    | Present              |                        |
| `Block`, `Inline`                          | `0`             | `1`                    | Present              |                        |
| `Block`, `HtmlAttributes`                  | `0`             | `1`                    | Present              |                        |
| `Block`, `Any`                             | `0`             | `1`                    | Present              |                        |
| `HtmlAttributes`, `Any`                    | `0`             | `1`                    | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `0`             | `1`                    | Present              |                        |
| `Block`, `Inline`, `Any`                   | `0`             | `1`                    | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | `0`             | `1`                    | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `0`             | `1`                    | Present              |                        |
| `Block`                                    | `0`             | `> 1`                  | Present              |                        |
| `HtmlAttributes`                           | `0`             | `> 1`                  | Present              |                        |
| `Any`                                      | `0`             | `> 1`                  | Present              |                        |
| `Block`, `Inline`                          | `0`             | `> 1`                  | Present              |                        |
| `Block`, `HtmlAttributes`                  | `0`             | `> 1`                  | Present              |                        |
| `Block`, `Any`                             | `0`             | `> 1`                  | Present              |                        |
| `HtmlAttributes`, `Any`                    | `0`             | `> 1`                  | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `0`             | `> 1`                  | Present              |                        |
| `Block`, `Inline`, `Any`                   | `0`             | `> 1`                  | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | `0`             | `> 1`                  | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `0`             | `> 1`                  | Present              |                        |
| `Block`                                    | `1`             | `1`                    | Present              |                        |
| `HtmlAttributes`                           | `1`             | `1`                    | Present              |                        |
| `Any`                                      | `1`             | `1`                    | Present              |                        |
| `Block`, `Inline`                          | `1`             | `1`                    | Present              |                        |
| `Block`, `HtmlAttributes`                  | `1`             | `1`                    | Present              |                        |
| `Block`, `Any`                             | `1`             | `1`                    | Present              |                        |
| `HtmlAttributes`, `Any`                    | `1`             | `1`                    | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `1`             | `1`                    | Present              |                        |
| `Block`, `Inline`, `Any`                   | `1`             | `1`                    | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | `1`             | `1`                    | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `1`             | `1`                    | Present              |                        |
| `Block`                                    | `1`             | `> 1`                  | Present              |                        |
| `HtmlAttributes`                           | `1`             | `> 1`                  | Present              |                        |
| `Any`                                      | `1`             | `> 1`                  | Present              |                        |
| `Block`, `Inline`                          | `1`             | `> 1`                  | Present              |                        |
| `Block`, `HtmlAttributes`                  | `1`             | `> 1`                  | Present              |                        |
| `Block`, `Any`                             | `1`             | `> 1`                  | Present              |                        |
| `HtmlAttributes`, `Any`                    | `1`             | `> 1`                  | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `1`             | `> 1`                  | Present              |                        |
| `Block`, `Inline`, `Any`                   | `1`             | `> 1`                  | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | `1`             | `> 1`                  | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `1`             | `> 1`                  | Present              |                        |
| `Block`                                    | `> 1`           | `MaxDepth == MinDepth` | Present              |                        |
| `HtmlAttributes`                           | `> 1`           | `MaxDepth == MinDepth` | Present              |                        |
| `Any`                                      | `> 1`           | `MaxDepth == MinDepth` | Present              |                        |
| `Block`, `Inline`                          | `> 1`           | `MaxDepth == MinDepth` | Present              |                        |
| `Block`, `HtmlAttributes`                  | `> 1`           | `MaxDepth == MinDepth` | Present              |                        |
| `Block`, `Any`                             | `> 1`           | `MaxDepth == MinDepth` | Present              |                        |
| `HtmlAttributes`, `Any`                    | `> 1`           | `MaxDepth == MinDepth` | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `> 1`           | `MaxDepth == MinDepth` | Present              |                        |
| `Block`, `Inline`, `Any`                   | `> 1`           | `MaxDepth == MinDepth` | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | `> 1`           | `MaxDepth == MinDepth` | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `> 1`           | `MaxDepth == MinDepth` | Present              |                        |
| `Block`                                    | `> 1`           | `MaxDepth > MinDepth`  | Present              |                        |
| `HtmlAttributes`                           | `> 1`           | `MaxDepth > MinDepth`  | Present              |                        |
| `Any`                                      | `> 1`           | `MaxDepth > MinDepth`  | Present              |                        |
| `Block`, `Inline`                          | `> 1`           | `MaxDepth > MinDepth`  | Present              |                        |
| `Block`, `HtmlAttributes`                  | `> 1`           | `MaxDepth > MinDepth`  | Present              |                        |
| `Block`, `Any`                             | `> 1`           | `MaxDepth > MinDepth`  | Present              |                        |
| `HtmlAttributes`, `Any`                    | `> 1`           | `MaxDepth > MinDepth`  | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `> 1`           | `MaxDepth > MinDepth`  | Present              |                        |
| `Block`, `Inline`, `Any`                   | `> 1`           | `MaxDepth > MinDepth`  | Present              |                        |
| `Block`, `HtmlAttributes`, `Any`           | `> 1`           | `MaxDepth > MinDepth`  | Present              |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `> 1`           | `MaxDepth > MinDepth`  | Present              |                        |

#### DepthRange Parameter Set

| Type                                       | MinDepth        | MaxDepth        | IncludeAttributes | Input Object Processor |
| ------------------------------------------ | --------------- | --------------- | ----------------- | ---------------------- |
| *(not present)*                            | *(not present)* | *(not present)* | *(not present)*   |                        |
| `Block`                                    | *(not present)* | *(not present)* | *(not present)*   |                        |
| `HtmlAttributes`                           | *(not present)* | *(not present)* | *(not present)*   |                        |
| `Any`                                      | *(not present)* | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`                          | *(not present)* | *(not present)* | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | *(not present)* | *(not present)* | *(not present)*   |                        |
| `Block`, `Any`                             | *(not present)* | *(not present)* | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | *(not present)* | *(not present)* | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | *(not present)* | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | *(not present)* | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | *(not present)* | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | *(not present)* | *(not present)* | *(not present)*   |                        |
| *(not present)*                            | `0`             | *(not present)* | *(not present)*   |                        |
| *(not present)*                            | `1`             | *(not present)* | *(not present)*   |                        |
| *(not present)*                            | `> 1`           | *(not present)* | *(not present)*   |                        |
| `Block`                                    | `0`             | *(not present)* | *(not present)*   |                        |
| `HtmlAttributes`                           | `0`             | *(not present)* | *(not present)*   |                        |
| `Any`                                      | `0`             | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`                          | `0`             | *(not present)* | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `0`             | *(not present)* | *(not present)*   |                        |
| `Block`, `Any`                             | `0`             | *(not present)* | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | `0`             | *(not present)* | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `0`             | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `0`             | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `0`             | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `0`             | *(not present)* | *(not present)*   |                        |
| `Block`                                    | `1`             | *(not present)* | *(not present)*   |                        |
| `HtmlAttributes`                           | `1`             | *(not present)* | *(not present)*   |                        |
| `Any`                                      | `1`             | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`                          | `1`             | *(not present)* | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `1`             | *(not present)* | *(not present)*   |                        |
| `Block`, `Any`                             | `1`             | *(not present)* | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | `1`             | *(not present)* | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `1`             | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `1`             | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `1`             | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `1`             | *(not present)* | *(not present)*   |                        |
| `Block`                                    | `> 1`           | *(not present)* | *(not present)*   |                        |
| `HtmlAttributes`                           | `> 1`           | *(not present)* | *(not present)*   |                        |
| `Any`                                      | `> 1`           | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`                          | `> 1`           | *(not present)* | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `> 1`           | *(not present)* | *(not present)*   |                        |
| `Block`, `Any`                             | `> 1`           | *(not present)* | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | `> 1`           | *(not present)* | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `> 1`           | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `> 1`           | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `> 1`           | *(not present)* | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `> 1`           | *(not present)* | *(not present)*   |                        |
| *(not present)*                            | *(not present)* | `0`             | *(not present)*   |                        |
| *(not present)*                            | *(not present)* | `1`             | *(not present)*   |                        |
| *(not present)*                            | *(not present)* | `> 1`           | *(not present)*   |                        |
| `Block`                                    | *(not present)* | `0`             | *(not present)*   |                        |
| `HtmlAttributes`                           | *(not present)* | `0`             | *(not present)*   |                        |
| `Any`                                      | *(not present)* | `0`             | *(not present)*   |                        |
| `Block`, `Inline`                          | *(not present)* | `0`             | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | *(not present)* | `0`             | *(not present)*   |                        |
| `Block`, `Any`                             | *(not present)* | `0`             | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | *(not present)* | `0`             | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | *(not present)* | `0`             | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | *(not present)* | `0`             | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | *(not present)* | `0`             | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | *(not present)* | `0`             | *(not present)*   |                        |
| `Block`                                    | *(not present)* | `1`             | *(not present)*   |                        |
| `HtmlAttributes`                           | *(not present)* | `1`             | *(not present)*   |                        |
| `Any`                                      | *(not present)* | `1`             | *(not present)*   |                        |
| `Block`, `Inline`                          | *(not present)* | `1`             | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | *(not present)* | `1`             | *(not present)*   |                        |
| `Block`, `Any`                             | *(not present)* | `1`             | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | *(not present)* | `1`             | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | *(not present)* | `1`             | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | *(not present)* | `1`             | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | *(not present)* | `1`             | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | *(not present)* | `1`             | *(not present)*   |                        |
| `Block`                                    | *(not present)* | `> 1`           | *(not present)*   |                        |
| `HtmlAttributes`                           | *(not present)* | `> 1`           | *(not present)*   |                        |
| `Any`                                      | *(not present)* | `> 1`           | *(not present)*   |                        |
| `Block`, `Inline`                          | *(not present)* | `> 1`           | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | *(not present)* | `> 1`           | *(not present)*   |                        |
| `Block`, `Any`                             | *(not present)* | `> 1`           | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | *(not present)* | `> 1`           | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | *(not present)* | `> 1`           | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | *(not present)* | `> 1`           | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | *(not present)* | `> 1`           | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | *(not present)* | `> 1`           | *(not present)*   |                        |
| *(not present)*                            | `0`             | `0`             | *(not present)*   |                        |
| *(not present)*                            | `0`             | `1`             | *(not present)*   |                        |
| *(not present)*                            | `0`             | `> 1`           | *(not present)*   |                        |
| *(not present)*                            | `1`             | `1`             | *(not present)*   |                        |
| *(not present)*                            | `1`             | `> 1`           | *(not present)*   |                        |
| *(not present)*                            | `> 1`           | `== MinDepth`   | *(not present)*   |                        |
| *(not present)*                            | `> 1`           | `> MinDepth`    | *(not present)*   |                        |
| `Block`                                    | `0`             | `0`             | *(not present)*   |                        |
| `HtmlAttributes`                           | `0`             | `0`             | *(not present)*   |                        |
| `Any`                                      | `0`             | `0`             | *(not present)*   |                        |
| `Block`, `Inline`                          | `0`             | `0`             | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `0`             | `0`             | *(not present)*   |                        |
| `Block`, `Any`                             | `0`             | `0`             | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | `0`             | `0`             | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `0`             | `0`             | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `0`             | `0`             | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `0`             | `0`             | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `0`             | `0`             | *(not present)*   |                        |
| `Block`                                    | `0`             | `1`             | *(not present)*   |                        |
| `HtmlAttributes`                           | `0`             | `1`             | *(not present)*   |                        |
| `Any`                                      | `0`             | `1`             | *(not present)*   |                        |
| `Block`, `Inline`                          | `0`             | `1`             | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `0`             | `1`             | *(not present)*   |                        |
| `Block`, `Any`                             | `0`             | `1`             | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | `0`             | `1`             | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `0`             | `1`             | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `0`             | `1`             | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `0`             | `1`             | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `0`             | `1`             | *(not present)*   |                        |
| `Block`                                    | `0`             | `> 1`           | *(not present)*   |                        |
| `HtmlAttributes`                           | `0`             | `> 1`           | *(not present)*   |                        |
| `Any`                                      | `0`             | `> 1`           | *(not present)*   |                        |
| `Block`, `Inline`                          | `0`             | `> 1`           | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `0`             | `> 1`           | *(not present)*   |                        |
| `Block`, `Any`                             | `0`             | `> 1`           | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | `0`             | `> 1`           | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `0`             | `> 1`           | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `0`             | `> 1`           | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `0`             | `> 1`           | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `0`             | `> 1`           | *(not present)*   |                        |
| `Block`                                    | `1`             | `1`             | *(not present)*   |                        |
| `HtmlAttributes`                           | `1`             | `1`             | *(not present)*   |                        |
| `Any`                                      | `1`             | `1`             | *(not present)*   |                        |
| `Block`, `Inline`                          | `1`             | `1`             | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `1`             | `1`             | *(not present)*   |                        |
| `Block`, `Any`                             | `1`             | `1`             | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | `1`             | `1`             | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `1`             | `1`             | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `1`             | `1`             | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `1`             | `1`             | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `1`             | `1`             | *(not present)*   |                        |
| `Block`                                    | `1`             | `> 1`           | *(not present)*   |                        |
| `HtmlAttributes`                           | `1`             | `> 1`           | *(not present)*   |                        |
| `Any`                                      | `1`             | `> 1`           | *(not present)*   |                        |
| `Block`, `Inline`                          | `1`             | `> 1`           | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `1`             | `> 1`           | *(not present)*   |                        |
| `Block`, `Any`                             | `1`             | `> 1`           | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | `1`             | `> 1`           | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `1`             | `> 1`           | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `1`             | `> 1`           | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `1`             | `> 1`           | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `1`             | `> 1`           | *(not present)*   |                        |
| `Block`                                    | `> 1`           | `== MinDepth`   | *(not present)*   |                        |
| `HtmlAttributes`                           | `> 1`           | `== MinDepth`   | *(not present)*   |                        |
| `Any`                                      | `> 1`           | `== MinDepth`   | *(not present)*   |                        |
| `Block`, `Inline`                          | `> 1`           | `== MinDepth`   | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `> 1`           | `== MinDepth`   | *(not present)*   |                        |
| `Block`, `Any`                             | `> 1`           | `== MinDepth`   | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | `> 1`           | `== MinDepth`   | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `> 1`           | `== MinDepth`   | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `> 1`           | `== MinDepth`   | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `> 1`           | `== MinDepth`   | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `> 1`           | `== MinDepth`   | *(not present)*   |                        |
| `Block`                                    | `> 1`           | `> MinDepth`    | *(not present)*   |                        |
| `HtmlAttributes`                           | `> 1`           | `> MinDepth`    | *(not present)*   |                        |
| `Any`                                      | `> 1`           | `> MinDepth`    | *(not present)*   |                        |
| `Block`, `Inline`                          | `> 1`           | `> MinDepth`    | *(not present)*   |                        |
| `Block`, `HtmlAttributes`                  | `> 1`           | `> MinDepth`    | *(not present)*   |                        |
| `Block`, `Any`                             | `> 1`           | `> MinDepth`    | *(not present)*   |                        |
| `HtmlAttributes`, `Any`                    | `> 1`           | `> MinDepth`    | *(not present)*   |                        |
| `Block`, `HtmlAttributes`, `Any`           | `> 1`           | `> MinDepth`    | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`        | `> 1`           | `> MinDepth`    | *(not present)*   |                        |
| `Block`, `Inline`, `Any`                   | `> 1`           | `> MinDepth`    | *(not present)*   |                        |
| `Block`, `Inline`, `HtmlAttributes`, `Any` | `> 1`           | `> MinDepth`    | *(not present)*   |                        |
| *(not present)*                            | *(not present)* | *(not present)* | Present           |                        |
| `Block`                                    | *(not present)* | *(not present)* | Present           |                        |
| `Any`                                      | *(not present)* | *(not present)* | Present           |                        |
| `Block`, `Inline`                          | *(not present)* | *(not present)* | Present           |                        |
| `Block`, `Any`                             | *(not present)* | *(not present)* | Present           |                        |
| `Block`, `Inline`, `Any`                   | *(not present)* | *(not present)* | Present           |                        |
| *(not present)*                            | `0`             | *(not present)* | Present           |                        |
| *(not present)*                            | `1`             | *(not present)* | Present           |                        |
| *(not present)*                            | `> 1`           | *(not present)* | Present           |                        |
| `Block`                                    | `0`             | *(not present)* | Present           |                        |
| `HtmlAttributes`                           | `0`             | *(not present)* | Present           |                        |
| `Any`                                      | `0`             | *(not present)* | Present           |                        |
| `Block`, `Inline`                          | `0`             | *(not present)* | Present           |                        |
| `Block`, `Any`                             | `0`             | *(not present)* | Present           |                        |
| `Block`, `Inline`, `Any`                   | `0`             | *(not present)* | Present           |                        |
| `Block`                                    | `1`             | *(not present)* | Present           |                        |
| `HtmlAttributes`                           | `1`             | *(not present)* | Present           |                        |
| `Any`                                      | `1`             | *(not present)* | Present           |                        |
| `Block`, `Inline`                          | `1`             | *(not present)* | Present           |                        |
| `Block`, `Any`                             | `1`             | *(not present)* | Present           |                        |
| `Block`, `Inline`, `Any`                   | `1`             | *(not present)* | Present           |                        |
| `Block`                                    | `> 1`           | *(not present)* | Present           |                        |
| `HtmlAttributes`                           | `> 1`           | *(not present)* | Present           |                        |
| `Any`                                      | `> 1`           | *(not present)* | Present           |                        |
| `Block`, `Inline`                          | `> 1`           | *(not present)* | Present           |                        |
| `Block`, `Any`                             | `> 1`           | *(not present)* | Present           |                        |
| `Block`, `Inline`, `Any`                   | `> 1`           | *(not present)* | Present           |                        |
| *(not present)*                            | *(not present)* | `0`             | Present           |                        |
| *(not present)*                            | *(not present)* | `1`             | Present           |                        |
| *(not present)*                            | *(not present)* | `> 1`           | Present           |                        |
| `Block`                                    | *(not present)* | `0`             | Present           |                        |
| `HtmlAttributes`                           | *(not present)* | `0`             | Present           |                        |
| `Any`                                      | *(not present)* | `0`             | Present           |                        |
| `Block`, `Inline`                          | *(not present)* | `0`             | Present           |                        |
| `Block`, `Any`                             | *(not present)* | `0`             | Present           |                        |
| `Block`, `Inline`, `Any`                   | *(not present)* | `0`             | Present           |                        |
| `Block`                                    | *(not present)* | `1`             | Present           |                        |
| `HtmlAttributes`                           | *(not present)* | `1`             | Present           |                        |
| `Any`                                      | *(not present)* | `1`             | Present           |                        |
| `Block`, `Inline`                          | *(not present)* | `1`             | Present           |                        |
| `Block`, `Any`                             | *(not present)* | `1`             | Present           |                        |
| `Block`, `Inline`, `Any`                   | *(not present)* | `1`             | Present           |                        |
| `Block`                                    | *(not present)* | `> 1`           | Present           |                        |
| `HtmlAttributes`                           | *(not present)* | `> 1`           | Present           |                        |
| `Any`                                      | *(not present)* | `> 1`           | Present           |                        |
| `Block`, `Inline`                          | *(not present)* | `> 1`           | Present           |                        |
| `Block`, `Any`                             | *(not present)* | `> 1`           | Present           |                        |
| `Block`, `Inline`, `Any`                   | *(not present)* | `> 1`           | Present           |                        |
| *(not present)*                            | `0`             | `0`             | Present           |                        |
| *(not present)*                            | `0`             | `1`             | Present           |                        |
| *(not present)*                            | `0`             | `> 1`           | Present           |                        |
| *(not present)*                            | `1`             | `1`             | Present           |                        |
| *(not present)*                            | `1`             | `> 1`           | Present           |                        |
| *(not present)*                            | `> 1`           | `== MinDepth`   | Present           |                        |
| *(not present)*                            | `> 1`           | `> MinDepth`    | Present           |                        |
| `Block`                                    | `0`             | `0`             | Present           |                        |
| `HtmlAttributes`                           | `0`             | `0`             | Present           |                        |
| `Any`                                      | `0`             | `0`             | Present           |                        |
| `Block`, `Inline`                          | `0`             | `0`             | Present           |                        |
| `Block`, `Any`                             | `0`             | `0`             | Present           |                        |
| `Block`, `Inline`, `Any`                   | `0`             | `0`             | Present           |                        |
| `Block`                                    | `0`             | `1`             | Present           |                        |
| `HtmlAttributes`                           | `0`             | `1`             | Present           |                        |
| `Any`                                      | `0`             | `1`             | Present           |                        |
| `Block`, `Inline`                          | `0`             | `1`             | Present           |                        |
| `Block`, `Any`                             | `0`             | `1`             | Present           |                        |
| `Block`, `Inline`, `Any`                   | `0`             | `1`             | Present           |                        |
| `Block`                                    | `0`             | `> 1`           | Present           |                        |
| `HtmlAttributes`                           | `0`             | `> 1`           | Present           |                        |
| `Any`                                      | `0`             | `> 1`           | Present           |                        |
| `Block`, `Inline`                          | `0`             | `> 1`           | Present           |                        |
| `Block`, `Any`                             | `0`             | `> 1`           | Present           |                        |
| `Block`, `Inline`, `Any`                   | `0`             | `> 1`           | Present           |                        |
| `Block`                                    | `1`             | `1`             | Present           |                        |
| `HtmlAttributes`                           | `1`             | `1`             | Present           |                        |
| `Any`                                      | `1`             | `1`             | Present           |                        |
| `Block`, `Inline`                          | `1`             | `1`             | Present           |                        |
| `Block`, `Any`                             | `1`             | `1`             | Present           |                        |
| `Block`, `Inline`, `Any`                   | `1`             | `1`             | Present           |                        |
| `Block`                                    | `1`             | `> 1`           | Present           |                        |
| `HtmlAttributes`                           | `1`             | `> 1`           | Present           |                        |
| `Any`                                      | `1`             | `> 1`           | Present           |                        |
| `Block`, `Inline`                          | `1`             | `> 1`           | Present           |                        |
| `Block`, `Any`                             | `1`             | `> 1`           | Present           |                        |
| `Block`, `Inline`, `Any`                   | `1`             | `> 1`           | Present           |                        |
| `Block`                                    | `> 1`           | `== MinDepth`   | Present           |                        |
| `HtmlAttributes`                           | `> 1`           | `== MinDepth`   | Present           |                        |
| `Any`                                      | `> 1`           | `== MinDepth`   | Present           |                        |
| `Block`, `Inline`                          | `> 1`           | `== MinDepth`   | Present           |                        |
| `Block`, `Any`                             | `> 1`           | `== MinDepth`   | Present           |                        |
| `Block`, `Inline`, `Any`                   | `> 1`           | `== MinDepth`   | Present           |                        |
| `Block`                                    | `> 1`           | `> MinDepth`    | Present           |                        |
| `HtmlAttributes`                           | `> 1`           | `> MinDepth`    | Present           |                        |
| `Any`                                      | `> 1`           | `> MinDepth`    | Present           |                        |
| `Block`, `Inline`                          | `> 1`           | `> MinDepth`    | Present           |                        |
| `Block`, `Any`                             | `> 1`           | `> MinDepth`    | Present           |                        |
| `Block`, `Inline`, `Any`                   | `> 1`           | `> MinDepth`    | Present           |                        |
