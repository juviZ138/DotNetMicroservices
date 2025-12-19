"use client";
import Image from "next/image";
import { useState } from "react";

type Props = {
  imageUrl: string;
};

export default function CardImage({ imageUrl }: Props) {
  const [loading, setLoading] = useState(true);

  return (
    <Image
      src={imageUrl}
      alt="image of car"
      fill
      className={`object-cover duration-700  ease-in-out ${
        loading ? "opaciy-0 scale-110" : "opacity-100 scale-100"
      }`}
      priority
      sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 25vw"
      onLoad={() => {
        setLoading(false);
      }}
    />
  );
}
