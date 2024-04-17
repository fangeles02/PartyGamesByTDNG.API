echo "Stopping kestrel service (if running)";
sudo systemctl stop kestrel-wERP-main.service;
echo "Disabling kestrel service";
sudo systemctl disable kestrel-wERP-main.service;
echo "Removing existing files...";
sudo rm /etc/systemd/system/kestrel-wERP-main.service;
sudo systemctl daemon-reload;