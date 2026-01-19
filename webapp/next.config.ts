import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // logging.fetches.fullUrl: Next.js'in fetch çağrılarını loglarken tam URL'yi göstermesini sağlar.
  // Development ortamında hangi endpoint'lere istek yapıldığını görmek ve debug etmek için kullanışlıdır.
  logging:{
    fetches:{
      fullUrl:true
    }
  },
  reactCompiler: true,
};

export default nextConfig;
