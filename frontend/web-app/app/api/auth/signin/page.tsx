import EmptyFilter from "@/app/components/EmptyFilter";

export default async function SingIn({
  searchParams,
}: {
  searchParams: Promise<{ callbackUrl: string }>;
}) {
  const { callbackUrl } = await searchParams;
  return (
    <EmptyFilter
      title="You need to be login to do that"
      subtitle="Please click below to login"
      showLogin
      callbackUrl={callbackUrl}
    />
  );
}
