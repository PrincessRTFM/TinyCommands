build: restore debug release ## Compile for both debug and release builds

release: restore ## Compile a release build
	dotnet.exe build --no-restore -c Release

debug: restore ## Compile a debug build
	dotnet.exe build --no-restore -c Debug

restore: ## Restore and verify nuget packages, dependencies, and tools in preparation for a build
	dotnet.exe restore -r win

# Follow any target line with two hashes and the description/helpline and `make help` will autogenerate for them
help:
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-16s\033[0m %s\n", $$1, $$2}'

.PHONY: restore debug release build help
