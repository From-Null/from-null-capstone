#!/usr/bin/env python3
"""Generate a Mermaid class diagram from simple C# classes.

Usage: tools/class-diagram.py
"""

import re
import sys
from pathlib import Path

CLASS = re.compile(r"public class (\w+)(?:\s*:\s*(\w+))?")
FIELD = re.compile(r"public (\w+) (\w+)\s*(?:;|=>)")
CONSTRUCTOR = re.compile(r"public (\w+)\((.*)\)")
METHOD = re.compile(r"public ([\w<>]+) (\w+)\((.*)\)")
ATTRIBUTES = re.compile(r"^(\[.*?\]\s*)+")  # e.g., [TextArea] before a field


def mermaid_type(type_):
    """Mermaid writes generics as List~Cell~ instead of List<Cell>."""
    return type_.replace("<", "~").replace(">", "~")


def param_types(params):
    return [mermaid_type(p.split()[0]) for p in params.split(",") if p.strip()]


def parse(path):
    """Return (name, base, fields, constructors, methods) for the class in a file."""
    name = base = None
    fields = []  # (type, name)
    constructors = []  # list of parameter types
    methods = []  # (return type, name, parameter types)
    for line in path.read_text().splitlines():
        line = ATTRIBUTES.sub("", line.strip())
        if m := CLASS.match(line):
            name, base = m.groups()
        elif name and (m := CONSTRUCTOR.match(line)) and m[1] == name:
            constructors.append(param_types(m[2]))
        elif m := METHOD.match(line):
            methods.append((mermaid_type(m[1]), m[2], param_types(m[3])))
        elif m := FIELD.match(line):
            fields.append((m[1], m[2]))
    return name, base, fields, constructors, methods


def diagram(classes):
    out = ["```mermaid", "classDiagram"]
    names = {c[0] for c in classes}
    arrows = []
    for name, base, fields, constructors, methods in classes:
        out.append(f"    class {name} {{")
        if base:
            out.append(f"        <<{base}>>")
        for type_, field in fields:
            out.append(f"        +{type_} {field}")
        for params in constructors:
            out.append(f"        +{name}({', '.join(params)})")
        for return_type, method, params in methods:
            out.append(f"        +{method}({', '.join(params)}) {return_type}")
        out.append("    }")
        arrows += [f"    {name} --> {t} : {f}" for t, f in fields if t in names]
    return out + arrows + ["```"]


def main(dirs):
    files = sorted(f for d in dirs for f in Path(d).rglob("*.cs"))
    classes = sorted((c for c in map(parse, files) if c[0]), key=lambda c: c[0])
    print("\n".join(diagram(classes)))


if __name__ == "__main__":
    main(sys.argv[1:] or ["Assets/Scripts/Data"])
