build: debug release

debug:
	dotnet.exe restore -r win
	dotnet.exe build --no-restore -c Debug

release:
	dotnet.exe restore -r win
	dotnet.exe build --no-restore -c Release

commit: build
	git add --all
	git commit

push: commit
	git push

.PHONY: build debug release commit push

