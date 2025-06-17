#!/bin/bash
set -e

# Update package lists
sudo apt-get update

# Install required dependencies
sudo apt-get install -y wget apt-transport-https software-properties-common

# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Update package lists after adding Microsoft repository
sudo apt-get update

# Install .NET 8.0 SDK
sudo apt-get install -y dotnet-sdk-8.0

# Verify .NET installation
dotnet --version

# Navigate to the src directory and restore dependencies
cd /mnt/persist/workspace/src
dotnet restore

# Add dotnet to PATH in user profile
echo 'export PATH="$PATH:/usr/share/dotnet"' >> $HOME/.profile

# Source the profile to make dotnet available in current session
export PATH="$PATH:/usr/share/dotnet"

echo "Setup completed successfully!"