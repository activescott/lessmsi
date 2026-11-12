These tests run the Python.NET wrapper against the real MSI extraction library
and the repository's existing MSI fixture. They extract files into temporary
directories; they do not install an MSI.

Build the normal Release/x86 binaries first, then use a 32-bit Python environment
with `pythonnet` installed. Install `tqdm` to also exercise the optional progress
bar wrapper. Point `LESSMSI_TEST_ASSEMBLY_DIR` at the built directory containing
`lessmsi.core.dll`, `LessIO.dll`, `libmspackn.dll` and `mspack.dll`.

From the repository root:

```console
python -m unittest discover -s contrib/python/tests -v
```
