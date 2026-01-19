'use client';
import { searchQuestions } from "@/lib/actions/question-action";
import { Question } from "@/lib/types";
import { MagnifyingGlassIcon } from "@heroicons/react/24/solid";
import { Input, Listbox, ListboxItem } from "@heroui/react";
import { useEffect, useRef, useState } from "react";


export default function SearchInput() {

    // State yönetimi: Kullanıcının yazdığı arama metni, yükleme durumu, sonuçlar ve dropdown görünürlüğü
    const [query,setQuery] = useState('');

    const [loading,setLoading] = useState(false);

    const [results,setResults]=useState<Question[] | null>([]);

    const [showDropdown,setShowDropdown] = useState(false);

    // useRef: Component re-render'larında değişmeyen, timeout ID'sini saklamak için kullanılır.
    // useRef kullanmazsak her render'da yeni timeout oluşur ve önceki timeout'lar temizlenemez.
    const timeoutRef = useRef<NodeJS.Timeout | null>(null);

    // Debounce mekanizması: Kullanıcı her tuş vuruşunda API çağrısı yapmak yerine, 
    // 300ms bekleme süresi koyarak sadece kullanıcı yazmayı bıraktığında arama yapar.
    // Bu sayede gereksiz API çağrıları önlenir ve performans artar.
    useEffect(()=>{
        // Önceki timeout'u temizle: Eğer kullanıcı hızlı yazıyorsa, önceki arama işlemini iptal et.
        if(timeoutRef.current) clearTimeout(timeoutRef.current);

        // Eğer query boşsa, sonuçları temizle ve dropdown'ı gizle.
        if(!query){
            // eslint-disable-next-line react-hooks/set-state-in-effect
            setResults(null);
            setShowDropdown(false);
            return;
        }

        // 300ms sonra arama yap: Kullanıcı 300ms boyunca yazmazsa, arama işlemini başlat.
        timeoutRef.current = setTimeout(async()=>{
            setLoading(true);
            try {
                const {data:questions, error} = await searchQuestions(query);
                if(error) {
                    console.error('Search error:', error);
                    setResults(null);
                    setShowDropdown(false);
                } else {
                    setResults(questions || null);
                    setShowDropdown(true);
                }
            } catch (err) {
                console.error('Search failed:', err);
                setResults(null);
                setShowDropdown(false);
            } finally {
                setLoading(false);
            }
        },300);

        // Cleanup: Component unmount olduğunda veya query değiştiğinde timeout'u temizle
        return () => {
            if(timeoutRef.current) {
                clearTimeout(timeoutRef.current);
            }
        };
    },[query])
    
    // C#/JS alternatifi: C#'da Timer veya CancellationTokenSource, JS'de setTimeout/clearTimeout ile
    // aynı mantık. Örnek JS: let timeoutId; input.addEventListener('input', (e) => { 
    // clearTimeout(timeoutId); timeoutId = setTimeout(() => search(e.target.value), 300); });
    

  const onAction = () =>{
    setQuery('');
    setResults(null);
  }

  return (
    <div className='flex flex-col w-full'>
          <Input
            startContent={<MagnifyingGlassIcon className="size-6"/>}
            type="search"
            placeholder="Search"
            className='ml-6' value={query} onChange={(e) => setQuery(e.target.value)}/>
            {showDropdown && results && (
                <div className='absolute top-full z-50 bg-white dark:bg-default-50 shadow-lg border-2 border-default-500 w-[50%]'>
                    <Listbox onAction={onAction} items={results || []} className='flex flex-col overflow-y-auto'>
                        {question => (
                            <ListboxItem 
                                href={`/questions/${question.id}`} 
                                key={question.id} 
                                aria-labelledby={question.title}
                                aria-describedby={question.content}
                                startContent={<div 
                                    className='flex flex-col 
                                    h-14 min-w-14 
                                    ustify-center items-center 
                                    border border-success rounded-md '>
                                        <span>{question.answerCount}</span>
                                        <span className='text-xs'>answers</span>
                                    </div>}
                              
                            >
                             <div>
                                <div className='font-semibold'>{question.title}</div>
                                <div className='text-xs opacity-60 line-clamp-2'> {question.content}</div>
                             </div>
                            </ListboxItem>

                  
                        )}
                    </Listbox>
                </div>
            )}
    </div>
    
  )
}
