echo "Stopping kestrel service (if running)";
sudo systemctl stop kestrel-partygamesbytdng-main.service;
echo "Disabling kestrel service";
sudo systemctl disable kestrel-partygamesbytdng-main.service;
echo "Removing existing files...";
sudo rm /etc/systemd/system/kestrel-partygamesbytdng-main.service;
sudo systemctl daemon-reload;