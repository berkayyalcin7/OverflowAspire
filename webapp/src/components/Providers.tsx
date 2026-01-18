'use client';

import { HeroUIProvider, ToastProvider } from "@heroui/react";
import { ThemeProvider } from "next-themes";
import { useRouter } from "next/navigation";
import { ReactNode } from "react";

export default function Providers({children} :{children: ReactNode}){

    const router = useRouter();

    return (
        <HeroUIProvider navigate={router.push} className="h-full flex flex-col">
            <ToastProvider />
            <ThemeProvider attribute='class' defaultTheme='light'>
                {children}
            </ThemeProvider>
        </HeroUIProvider>
    )
    
}

