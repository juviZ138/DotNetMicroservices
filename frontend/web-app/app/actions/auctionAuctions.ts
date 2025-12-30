"use server";

import { fetchWarpper } from "@/lib/fetchWarpper";
import { PagedResult, Auction, Bid } from "@/types";
import { FieldValues } from "react-hook-form";

export async function getData(query: string): Promise<PagedResult<Auction>> {
  return fetchWarpper.get(`search${query}`);
}

export async function updateAuctionTest(): Promise<{
  status: number;
  message: string;
}> {
  const data = {
    mileage: Math.floor(Math.random() * 10000) + 1,
  };

  return fetchWarpper.put(
    "auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c",
    data
  );
}

export async function createAuction(data: FieldValues) {
  return fetchWarpper.post("auctions", data);
}

export async function getDetailedViewData(id: string): Promise<Auction> {
  return fetchWarpper.get(`auctions/${id}`);
}

export async function updateAuction(data: FieldValues, id: string) {
  return fetchWarpper.put(`auctions/${id}`, data);
}

export async function deleteAuction(id: string) {
  return fetchWarpper.del(`auctions/${id}`);
}

export async function getBidsForAuction(id: string): Promise<Bid[]> {
  return fetchWarpper.get(`bids/${id}`);
}

export async function placeBidForAuction(auctionId: string, amount: number) {
  return fetchWarpper.post(`bids?auctionId=${auctionId}&amount=${amount}`, {});
}
