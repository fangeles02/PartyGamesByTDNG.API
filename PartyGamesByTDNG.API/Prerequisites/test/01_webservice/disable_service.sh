echo "Stopping kestrel service (if running)";
sudo systemctl stop kestrel-partygamesbytdng-test.service;
echo "Disabling kestrel service";
sudo systemctl disable kestrel-partygamesbytdng-test.service;
echo "Removing existing files...";
sudo rm /etc/systemd/system/kestrel-partygamesbytdng-test.service;
sudo systemctl daemon-reload;