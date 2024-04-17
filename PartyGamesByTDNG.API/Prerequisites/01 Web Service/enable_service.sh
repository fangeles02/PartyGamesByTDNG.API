sudo systemctl daemon-reload;
echo "Copying kestrel-partygamesbytdng-main.service to /etc/systemd/system";
sudo cp kestrel-partygamesbytdng-main.service /etc/systemd/system/kestrel-partygamesbytdng-main.service;
echo "Enabling kestrel-partygamesbytdng-main.service";
sudo systemctl enable kestrel-partygamesbytdng-main.service;
echo "Starting kestrel-partygamesbytdng-main.service";
sudo systemctl start kestrel-partygamesbytdng-main.service;
sudo systemctl status kestrel-partygamesbytdng-main.service;