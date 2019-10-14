Remove-Item "../Migrations" -Recurse -ErrorAction Ignore
Add-Migration InitialCreate -c KIOTContext
Add-Migration InitialCreate -c IdentityContext