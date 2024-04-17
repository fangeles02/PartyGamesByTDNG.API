sudo systemctl daemon-reload;
echo "Copying kestrel-wERP-main.service to /etc/systemd/system";
sudo cp kestrel-wERP-main.service /etc/systemd/system/kestrel-wERP-main.service;
echo "Enabling kestrel-wERP-main.service";
sudo systemctl enable kestrel-wERP-main.service;
echo "Starting kestrel-wERP-main.service";
sudo systemctl start kestrel-wERP-main.service;
sudo systemctl status kestrel-wERP-main.service;