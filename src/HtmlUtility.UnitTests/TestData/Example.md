# Example Markdown Document

[CommonMark Spec](https://spec.commonmark.org/0.31.2/)

Hard line break\
here

- [X] Task
- [ ] List Item

- Normal List
- Item

## Abbreviations

*[HTML]: Hyper Text Markup Language
*[W3C]:  World Wide Web Consortium

## Image with Alt and Title

![alt attribute goes here](./sn-logo.jpg "This is a Title" )

![foo *bar*]

[foo *bar*]: ./sn-logo.jpg "train & tracks"

## Math

This sentence uses `$` delimiters to show math inline: $\sqrt{3x-1}+(1+x)^2$

This sentence uses $\` and \`$ delimiters to show math inline: $`\sqrt{3x-1}+(1+x)^2`$

**The Cauchy-Schwarz Inequality**
$$\left( \sum_{k=1}^n a_k b_k \right)^2 \leq \left( \sum_{k=1}^n a_k^2 \right) \left( \sum_{k=1}^n b_k^2 \right)$$

## Definition List

Apple
:   Pomaceous fruit of plants of the genus Malus in
    the family Rosaceae.

Orange
:   The fruit of an evergreen tree of the genus Citrus.

## Code

This is `inline code`.

```log
This is a fenced code block
```

``` { .html #codeId style="color: #333; background: #f8f8f8;" }
This is a fenced code block
with an ID and style
```

## Attribute lists

This is red paragraph.
{: style="color: #333; color: #ff0000;" }

## Footnotes

Footnotes[^1] have a label[^@#$%] and the footnote's content.

[^1]: This is a footnote content.
[^@#$%]: A footnote on the label: "@#$%".

### Smarties

<< angle quotes >>

Ellipsis...
