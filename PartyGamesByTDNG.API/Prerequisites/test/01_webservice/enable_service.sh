sudo systemctl daemon-reload;
echo "Copying kestrel-partygamesbytdng-test.service to /etc/systemd/system";
sudo cp kestrel-partygamesbytdng-test.service /etc/systemd/system/kestrel-partygamesbytdng-test.service;
echo "Enabling kestrel-partygamesbytdng-test.service";
sudo systemctl enable kestrel-partygamesbytdng-test.service;
echo "Starting kestrel-partygamesbytdng-test.service";
sudo systemctl start kestrel-partygamesbytdng-test.service;
sudo systemctl status kestrel-partygamesbytdng-test.service;