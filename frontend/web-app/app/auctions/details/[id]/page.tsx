import { getDetailedViewData } from "@/app/actions/auctionAuctions";
import Heading from "@/app/components/Heading";
import CountDownTimer from "../../CountDownTimer";
import CardImage from "../../CardImage";
import DetailedSpects from "./DetailedSpects";
import EditButton from "./EditButton";
import { getCurrentUser } from "@/app/actions/authAction";
import DeleteButton from "./DeleteButton";
import BidList from "./BidList";

export default async function Details({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const data = await getDetailedViewData(id);
  const user = await getCurrentUser();

  return (
    <>
      <div className="flex justify-between">
        <div className="flex items-center gap-3">
          <Heading title={`${data.make} ${data.model}`} />
          {user?.username === data.seller && (
            <>
              <EditButton id={data.id} /> <DeleteButton id={data.id} />
            </>
          )}
        </div>
        <div className="flex gap-3">
          <h3 className="text-2xl font-semibold">Time remaining: </h3>
          <CountDownTimer auctionEnd={data.auctionEnd} />
        </div>
      </div>
      <div className="grid grid-cols-2 gap-6 mt-3">
        <div className="relative w-full bg-gray-200 aspect-16/10 rounded-lg overflow-hidden">
          <CardImage imageUrl={data.imageUrl} />
        </div>
        <BidList user={user} auction={data} />
      </div>
      <div className="mt-3 grid grid-col-1 rounded-lg">
        <DetailedSpects auction={data} />
      </div>
    </>
  );
}
