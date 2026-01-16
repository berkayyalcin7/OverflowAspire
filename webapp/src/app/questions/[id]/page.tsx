import notFound from "@/app/not-found";
import { getQuestionById } from "@/lib/actions/question-action";
import QuestionDetailedHeader from "./QuestionDetailedHeader";
import QuestionContent from "./QuestionContent";
import AnswerContent from "./AnswerContent";
import AnswersHeader from "./AnswersHeader";

type Params = Promise<{id: string}>

export default async function QuestionDetailedPage(props: {params: Params}){
    const {id} = await props.params;

    const {data : question , error} = await getQuestionById(id);

    if(error){ throw error;}

    if(!question) {
        return notFound();
    }

    return (
        <div className="w-full">
            <QuestionDetailedHeader question={question} />
            <QuestionContent question={question} />
            {question.answers.length>0 && (
                <AnswersHeader answerCount={question.answers.length} />
            )}
            {question.answers.map(answer=>(
                <AnswerContent key={answer.id} answer={answer} />
            ))}
        </div>
    )
}