echo "copying project.assets.json from container"
cp -r /home/dotnet/EST.MIT.Web/obj/project.assets.json /home/dotnet/snyk/project.assets.json
echo "making project.assets.json writable by all so Jenkins can delete it"
chmod 666 /home/dotnet/snyk/project.assets.json