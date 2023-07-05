build: debug release ## Compile for both debug and release builds

release: ## Compile a release build
	dotnet.exe build -c Release

debug: ## Compile a debug build
	dotnet.exe build -c Debug

# Follow any target line with two hashes and the description/helpline and `make help` will autogenerate for them
help:
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-16s\033[0m %s\n", $$1, $$2}'

.PHONY: debug release build help
