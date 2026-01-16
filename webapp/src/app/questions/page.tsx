'use server';

import { getQuestions } from "@/lib/actions/question-action";
import QuestionCard from "./QuestionCard";
import QuestionsHeader from "./QuestionsHeader";

export default async function QuestionsPage({searchParams}:{searchParams?:Promise<{tag?:string}>}) {

    const params = await searchParams;

    const questions = await getQuestions(params?.tag);

    return (
        <>
            <QuestionsHeader total={questions.length} tag={params?.tag ?? ''}></QuestionsHeader>
                {questions.map(question=>(
                    <div key={question.id} className="py-4 not-last:border-b w-full flex">
                              <QuestionCard key={question.id} question={question} />
                              
                    </div>
              
                ))}
           
        </>
    )
}