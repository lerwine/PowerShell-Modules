# Example Heading 1

<!-- markdownlint-disable MD034 -->

Plain Text

[Heading Link](#example-heading)

This is red paragraph&darr;
{: style="color: #333; color: #ff0000;" }

Footnotes[^2] have a label[^@#$%] and the footnote's content, whereas [reference style links][1] have the URL followed by the text.

## Example Heading 2

### Example Heading 3

[CommonMark Spec](https://spec.commonmark.org/0.31.2/)

My favorite search engine is [Duck Duck Go](https://duckduckgo.com "The best search engine for privacy").

<https://www.markdownguide.org>
<fake@example.com>
https://github.com/lerwine/PowerShell-Modules

Soft line break
here

Hard line break\
here

- [X] Task
- [ ] List Item

- Normal List
- Item

1. Ordered
2. List

*[API]: Application Programming Interface
*[JSON]: JavaScript Object Notation

When working with APIs, it's common to send and receive data in JSON format.
This API uses JSON for all its responses.

> Block quoted
> *text*
>
> With a blank line

| Syntax      | Description |
| ----------- | ----------- |
| Header      | Title       |
| Paragraph   | Text        |

| Syntax      | Description | Test Text     |
| :---        |    :----:   |          ---: |
| Header      | Title       | Here's this   |
| Paragraph   | Text        | And more      |

## Example Heading 2{#example-heading}

![alt attribute goes here](./sn-logo.jpg "This is a Title" )

H~2~O, E = Mc^2^, $\sqrt{3x-1}+(1+x)^2$

This sentence uses $\` and \`$ delimiters to show math inline: $`\sqrt{3x-1}+(1+x)^2`$.

**The Cauchy-Schwarz Inequality**
$$\left( \sum_{k=1}^n a_k b_k \right)^2 \leq \left( \sum_{k=1}^n a_k^2 \right) \left( \sum_{k=1}^n b_k^2 \right)$$

This is `inline code`.

---

~~The world is flat.~~ That is ==so== funny!

```json
[ "This is a fenced code block" ]
```

``` { .html #codeId style="color: #333; background: #f8f8f8;" }
This is a fenced code block
with an ID and style
```

Apple
:   Pomaceous fruit of plants of the genus Malus in
    the family Rosaceae.

Orange
:   The fruit of an evergreen tree of the genus Citrus.

[1]: <https://github.com/xoofx/markdig/blob/master/readme.md> "Markdig Documentation"
[^2]: This is a footnote content.
[^@#$%]: A footnote on the label: "@#$%".
