sudo systemctl daemon-reload;
echo "Copying kestrel-partygamesbytdng-live.service to /etc/systemd/system";
sudo cp kestrel-partygamesbytdng-live.service /etc/systemd/system/kestrel-partygamesbytdng-live.service;
echo "Enabling kestrel-partygamesbytdng-live.service";
sudo systemctl enable kestrel-partygamesbytdng-live.service;
echo "Starting kestrel-partygamesbytdng-live.service";
sudo systemctl start kestrel-partygamesbytdng-live.service;
sudo systemctl status kestrel-partygamesbytdng-live.service;