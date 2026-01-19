'use client';

import { getTags } from "@/lib/actions/tag-actions";
import { useTagStore } from "@/lib/useTagStore";
import { HeroUIProvider, ToastProvider } from "@heroui/react";
import { ThemeProvider } from "next-themes";
import { useRouter } from "next/navigation";
import { ReactNode, useEffect } from "react";

export default function Providers({children} :{children: ReactNode}){

    const router = useRouter();

    // TagStore: Zustand ile global state management. Uygulama başlangıcında tag'leri bir kez yükleyip
    // global store'a kaydediyoruz. Bu sayede uygulamanın her yerinden (client component'lerde) 
    // tag'lere erişebiliyoruz, tekrar API çağrısı yapmadan getTagBySlug gibi metodlarla kullanabiliyoruz.
    const setTags = useTagStore((state)=>state.setTags);

    // useEffect: Component mount olduğunda (uygulama başladığında) bir kez çalışır. Async getTags fonksiyonunu
    // çağırıp tag'leri yükler. void loadTags() kullanımı: async fonksiyonu await etmeden çağırıyoruz çünkü
    // useEffect callback'i async olamaz. 
    // [setTags] dependency array: useEffect içinde kullanılan değişkenleri buraya ekleriz. setTags değiştiğinde
    // effect tekrar çalışır. Ancak setTags Zustand store'dan geldiği için stable'dır (değişmez), bu yüzden
    // effect sadece mount'ta bir kez çalışır. Boş array [] de kullanılabilir ama ESLint uyarısı vermemesi için eklenmiş.
    useEffect(()=>{
        const loadTags = async () =>{
            const {data:tags}= await getTags();
            if(tags) setTags(tags);
        }
        void loadTags();
    },[setTags])

    return (
        <HeroUIProvider navigate={router.push} className="h-full flex flex-col">
            <ToastProvider />
            <ThemeProvider attribute='class' defaultTheme='light'>
                {children}
            </ThemeProvider>
        </HeroUIProvider>
    )
    
}

