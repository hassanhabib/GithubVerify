# GithubVerify

A tool to help you configure and verify SSH-based commit signing for GitHub.  
Built with ❤️ by **The Standard Organization**, a coalition of the Good-Hearted Engineers.

Repository: [https://github.com/hassanhabib/GithubVerify](https://github.com/hassanhabib/GithubVerify)

---

## ✨ Features

- ✅ Check if Git commit signing is enabled and uses SSH
- 🔐 Generate SSH keys and configure Git signing
- 🔍 Verify that your latest commit is signed and valid
- ♻️ Reset the SSH commit signing configuration safely

---

## 🛠 Requirements

- Bash
- [Git](https://git-scm.com/downloads)
- OpenSSH (usually pre-installed on most systems)
- A [GitHub](https://github.com) account

---

## 🚀 Getting Started

### Clone and Build

```bash
git clone https://github.com/hassanhabib/GithubVerify.git
cd GithubVerify
```

### Run

```bash
./GithubVerify.sh [command] [username] [email]
```

---

## ⚙️ Commands

| Command   | Description                                                                 |
|-----------|-----------------------------------------------------------------------------|
| `check`   | Check if Git commit signing is enabled and using SSH                        |
| `setup`   | Generate SSH key, configure Git to sign commits using SSH                   |
| `verify`  | Verify that the latest Git commit is signed correctly                       |
| `reset`   | Reset the SSH signing configuration (removes keys and Git config changes)   |

---

## 📘 Examples

### Check commit signing status

```bash
./GithubVerify.sh check
```

### Setup SSH commit signing

```bash
./GithubVerify.sh setup hassan hassan@example.com
```

### Verify the latest commit signature

```bash
./GithubVerify.sh verify
```

### Reset all Git + SSH signing configuration

```bash
./GithubVerify.sh reset
```

---

## 🔐 What `setup` Does

- Generates `id_ed25519` key in `~/.ssh` if missing
- Creates `.gnupg/allowed_signers` file with your SSH public key
- Updates your global Git configuration with:
  - `gpg.format = ssh`
  - `user.signingkey = ~/.ssh/id_ed25519.pub`
  - `commit.gpgsign = true`
  - `gpg.ssh.allowedSignersFile = ~/.gnupg/allowed_signers`
- Instructs you to upload your public key to GitHub

---

## 🚨 Reset Behavior

The `reset` command will:

- Unset Git configuration values related to SSH commit signing
- Delete the `id_ed25519` and `id_ed25519.pub` files from `~/.ssh`
- Delete the `allowed_signers` file from `~/.gnupg`

> ⚠️ This is irreversible. Use only if you want to fully reset your signing configuration.

---

## 🔑 Upload Your Public Key

After running `setup`, you must upload your public key to GitHub:

```bash
cat ~/.ssh/id_ed25519.pub
```

Then go to: [https://github.com/settings/ssh](https://github.com/settings/ssh)

---

## 🧑‍💻 Developed By

**Hassan Habib**  

---

## 📄 License

The Standard Software License
Copyright © The Standard Organization  
See [LICENSE](https://github.com/hassanhabib/GithubVerify/blob/master/LICENSE) for full text.
