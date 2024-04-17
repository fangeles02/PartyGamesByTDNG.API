echo "Stopping kestrel service (if running)";
sudo systemctl stop kestrel-partygamesbytdng-live.service;
echo "Disabling kestrel service";
sudo systemctl disable kestrel-partygamesbytdng-live.service;
echo "Removing existing files...";
sudo rm /etc/systemd/system/kestrel-partygamesbytdng-live.service;
sudo systemctl daemon-reload;