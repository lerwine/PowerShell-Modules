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
