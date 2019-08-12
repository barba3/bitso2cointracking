# Bitso2Cointracking
A tool to export cryptocurrency transactions from Bitso to Cointracking in CSV format

## Set up
1. Create an API key in [Bitso website](https://help.bitso.com/en/support/solutions/articles/1000166794-tutorial-creating-an-api-entry). Read-only permissions are enough.
2. Enter the API key and secret in App.config file

## Use
1. Open a command prompt window
2. Run `Bitso2Cointracking.exe > data.csv`
3. Import data.csv into [Cointracking CSV Import](https://www.cointracking.info/import/import_csv/)  

# Known issues
- Unable to download more than 100 fundings or 100 withdrawals. Please consider contributing!
  - Add pagination support to fundings and withdrawals to [Bitso.Net](https://github.com/raulbojalil/bitso-dotnet) library.
  - Update this tool to use pagination.
  
# MIT License

Copyright (c) 2019 Bruno Barba Venturi

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
