"use client";

import { Dropdown, DropdownDivider, DropdownItem } from "flowbite-react";
import Link from "next/link";
import { User } from "next-auth";
import { HiCog, HiUser } from "react-icons/hi";
import { AiFillCar, AiFillTrophy, AiOutlineLogout } from "react-icons/ai";
import { signOut } from "next-auth/react";

type Props = {
  user: User;
};

export default function UserActions({ user }: Props) {
  return (
    <Dropdown inline label={`Welcome ${user.name}`} className="cursor-pointer">
      <DropdownItem icon={HiUser}>My auctions</DropdownItem>
      <DropdownItem icon={AiFillTrophy}>Auctions won</DropdownItem>
      <DropdownItem icon={AiFillCar}>Sell my car</DropdownItem>
      <DropdownItem icon={HiCog}>
        <Link href={"/session"}>Session (dev only!)</Link>
      </DropdownItem>
      <DropdownDivider />
      <DropdownItem
        icon={AiOutlineLogout}
        onClick={() => signOut({ redirectTo: "/" })}
      >
        Sign out
      </DropdownItem>
    </Dropdown>
  );
}
