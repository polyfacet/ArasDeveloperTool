# Release Notes

## Release 3.2.0

- Input command parsing improvement.
    If command input start matches a single command, run it. I.e. no need to be exact
- Adds connection timeout as parameter:
    Config or -connectionTimeout <int> # Number of seconds before a Aras request timeouts
- New Feature/Command: ApplyMethod
- (AMLRunner) Run as single aml(s) (xml) file
- Stabilizes login by making three tries, with a 10 sec delay between
- Other misc. fixes and improvements
