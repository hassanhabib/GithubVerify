#!/bin/bash

# ----------------------------------------------------------------------------------
# Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers
# ----------------------------------------------------------------------------------

SSHKEYPATH="$HOME/.ssh/id_ed25519"
ALLOWEDSIGNERSPATH="$HOME/.gnupg/allowed_signers"

case "$1" in
check)
  [[ $(git config --global --get commit.gpgsign) == "true" && $(git config --global --get gpg.format) == "ssh" ]] && echo "✅ Verified commit signing is enabled with SSH." || echo "❌ Verified commit signing is not configured properly."
  ;;
setup)
  [[ ! -f "$SSHKEYPATH" ]] && ssh-keygen -t ed25519 -C "${3:-your_email@example.com}" || echo "SSH key already exists at $SSHKEYPATH"
  echo ${2:-$USER} $(cat "$SSHKEYPATH.pub") >"$ALLOWEDSIGNERSPATH"
  git config --global gpg.format ssh
  git config --global user.signingkey "$SSHKEYPATH"
  git config --global commit.gpgsign true
  git config --global gpg.ssh.allowedSignersFile "$ALLOWEDSIGNERSPATH"
  echo -e "✅ SSH key and Git config setup completed.\n🔑 Upload your public key to GitHub: https://github.com/settings/ssh\n📋 Your public key is here: ~/.ssh/id_ed25519.pub"
  ;;
verify)
  gitlog=$(git log --show-signature -1)
  [[ $gitlog =~ "Good \"ssh-ed25519\" signature" ]] && echo "✅ Last commit is properly signed with SSH." && exit
  [[ $gitlog =~ "gpg.ssh.allowedSignersFile needs to be configured" ]] && echo "❌ Missing allowedSignersFile config. Run \`setup\` again or manually fix Git config." && exit
  [[ $gitlog =~ "No signature" ]] && echo "❌ Last commit is not signed." && exit
  echo -e "⚠️ Unknown verification state:\n$gitlog"
  ;;
reset)
  rm "$SSHKEYPATH" "$SSHKEYPATH.pub" "$ALLOWEDSIGNERSPATH"
  for o in gpg.format user.signingkey commit.gpgsign gpg.ssh.allowedSignersFile; do git config --global --unset "$o"; done
  ;;
selftest)
  bash $0 reset
  [[ -f "$SSHKEYPATH" ]] && echo "❌ WARNING TEST FAILED SSH KEY NOT DELETED" && exit 1
  ;;
hasan_minhajs_custom_feature)
  echo "Hello this is Hasan Minhaj's custom feature"
  ;;
*)
  echo "Usage: $0 [check|setup|verify|reset|selftest] [username] [email]"
  ;;
esac
