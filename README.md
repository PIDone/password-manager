# password-manager

A nice and simple command line password manager.

Installation steps:

1. Go to your home directory `cd ~`
2. Clone this repository `git clone https://github.com/PIDone/password-manager.git`
3. Create a new folder called Passwords `mkdir Passwords`
4. Enter the cloned repository `cd password-manager`
5. Move libpassword-manager and password-manager.pdb to the Passwords folder `mv *password-manager* ~/Passwords`
6. Move passwd-man to the home directory `mv passwd-man ~`
7. Go back to the home directory `cd ~`
8. Delete the cloned folder `rm -rf password-manager`
9. Edit the .bashrc file so that passwd-man is sourced `echo "source ~/passwd-man" >> .bashrc`
10. Exit the terminal `exit`
11. Once you reopen the terminal, you should be able to use the `passwd-man` command

The `passwd-man` command has to be called with arguments, eg `passwd-man set github username password`. For more information, write passwd-man help
